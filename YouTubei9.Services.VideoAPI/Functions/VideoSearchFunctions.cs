using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using YouTubei9.Services.VideoAPI.Data;
using YouTubei9.Services.VideoAPI.Models;
using YouTubei9.Services.VideoAPI.Models.DTO;
using YouTubei9.Services.VideoAPI.Models.VideoInfoComponents;
using YouTubei9.Services.VideoAPI.Models.VideoSearchComponents;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YouTubei9.Services.VideoAPI.Functions
{
    public class VideoSearchFunctions
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public VideoSearchFunctions(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<string> SaveAllYoutubeVideos(string apiKey)
        {
            var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q=dotnet8&type=video&relevanceLanguage=pt&publishedAfter=2025-01-01T00:00:00Z&maxResults=15&key={apiKey}";

            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var videos = JsonConvert.DeserializeObject<YTBVideoSearchDTO>(responseBody);


                var result = await SaveAllVideos(videos, apiKey);

                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("ERRO AO SE COMUNICAR COM A API: ", ex.Message);
            }
        }

        public async Task<string> GetYouTubeVideoDuration(string videoId, string apiKey)
        {
            var url = $"https://www.googleapis.com/youtube/v3/videos?id={videoId}&part=contentDetails&key={apiKey}";

            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var videoInfo = JsonConvert.DeserializeObject<YTBVideoInfoDTO>(responseBody);

                var videoDuration = ConvertDurationToString(videoInfo.Items[0].ContentDetails.Duration);

                return videoDuration;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("ERRO AO SE COMUNICAR COM A API: ", ex.Message);
            }
        }

        public string ConvertDurationToString(string videoDuration)
        {
            var regex = new Regex(@"PT(?:(\d+)H)?(?:(\d+)M)?(?:(\d+)S)?");
            var match = regex.Match(videoDuration);

            if (!match.Success)
                throw new ArgumentException("FORMATO INVÁLIDO!");

            int hours = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 0;
            int minutes = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;
            int seconds = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;

            TimeSpan timeSpan = new TimeSpan(hours, minutes, seconds);

            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        public List<YTBVideoSearch> GetVideos()
        {
            List<YTBVideoSearch> videosList = _db.YTBVideoSearches.Where(v => v.IsDeleted != true).ToList();
            List<ThumbnailItem> thumbsList = _db.YTBVideoSearchesThumbs.ToList();

            if(videosList == null || videosList.Count == 0 || thumbsList == null || thumbsList.Count == 0)
            {
                throw new ArgumentException("NÃO FORAM ENCONTRADOS VÍDEOS NO SEU BANCO DE DADOS!");
            }

            return videosList;
        }

        public async Task<YTBVideoSearch> GetVideoById(int id)
        {
            var video = await _db.YTBVideoSearches.Where(v => v.IsDeleted != true).FirstOrDefaultAsync(v => v.Id == id);

            if(video == null)
            {
                throw new ArgumentException("VÍDEO NÃO ENCONTRADO!");
            }

            return video;
        }

        public List<YTBVideoSearch> GetVideosByFilterFromDatabase(VideoFilters filter, string search)
        {
            var listVideos = GetVideos();
            var filteredVideos = new List<YTBVideoSearch>();
            search = search.ToLower();

            if (filter == VideoFilters.Title)
            {
                filteredVideos = listVideos
                    .Where(v => !string.IsNullOrEmpty(v.VideoTitle) && v.VideoTitle.ToLower()
                    .Contains(search))
                    .ToList();

                if (filteredVideos != null && filteredVideos.Count() > 0)
                {
                    return filteredVideos;
                }

                else throw new ArgumentException("NÃO FORAM ENCONTRADOS VÍDEOS COM ESTE TÍTULO!");
            }

            else if (filter == VideoFilters.Duration) 
            {
                if (search.Count(c => c == ':') == 1)
                {
                    search = $"00:{search}";
                }

                else if (search.Count(c => c == ':') == 0)
                {
                    search = $"00:00:{search}";
                }

                TimeSpan durationFilter = TimeSpan.Parse(search);

                filteredVideos = listVideos
                    .Where(v => !string.IsNullOrEmpty(v.Duration) &&
                    TimeSpan.TryParse(v.Duration, out TimeSpan videoDuration) &&
                    videoDuration <= durationFilter)
                    .ToList();

                if (filteredVideos != null && filteredVideos.Count() > 0)
                {
                    return filteredVideos;
                }

                else throw new ArgumentException("NÃO FORAM ENCONTRADOS VÍDEOS COM ATÉ ESTA DURAÇÃO DE TEMPO!");
            }

            else if (filter == VideoFilters.Author)
            {
                filteredVideos = listVideos
                    .Where(v => !string.IsNullOrEmpty(v.ChannelTitle) && v.ChannelTitle.ToLower()
                    .Contains(search))
                    .ToList();

                if (filteredVideos != null && filteredVideos.Count() > 0)
                {
                    return filteredVideos;
                }

                else throw new ArgumentException("NÃO FORAM ENCONTRADOS VÍDEOS DO AUTOR ESPECIFICADO!");
            }

            else if (filter == VideoFilters.ByDate)
            {
                if (DateTime.TryParseExact(search, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    if (parsedDate > DateTime.Now)
                    {
                        throw new ArgumentException("A DATA ULTRAPASSA O PERÍODO VÁLIDO!");
                    }
                    filteredVideos = listVideos
                    .Where(v => v.Date >= parsedDate && parsedDate.Year == 2025)
                    .ToList();

                    if (filteredVideos != null && filteredVideos.Count() > 0)
                    {
                        return filteredVideos;
                    }

                    else throw new ArgumentException("NÃO FORAM ENCONTRADOS VÍDEOS NO PERÍODO ESPECIFICADO!");
                }

                throw new ArgumentException("A DATA PRECISA ESTAR NO FORMATO dd-MM-yyyy!");

            }

            //A função manterá a lógica referente a ordem. Caso tenha apenas 1 termo, filtrará apenas título. Caso tenha 2, filtrará título e nome do canal
            //(TITULO, NOME_CANAL, DESCRICAO)
            else if (filter == VideoFilters.Q)
            {
                search = Regex.Replace(search, @"\s*,\s*", ",");

                var terms = search.Split(",");

                if (terms.Length < 1 || terms.Length > 3)
                {
                    throw new ArgumentException("QUANTIDADE INVÁLIDA DE TERMOS!");
                }

                var qSearch = (
                    Title: terms.ElementAtOrDefault(0),
                    ChannelT: terms.ElementAtOrDefault(1) ?? string.Empty,
                    Descrip: terms.ElementAtOrDefault(2) ?? string.Empty
                    );

                filteredVideos = listVideos
                    .Where(v =>
                        (string.IsNullOrEmpty(qSearch.Title) || 
                        v.VideoTitle?.Contains(qSearch.Title, StringComparison.OrdinalIgnoreCase) == true) &&
                        (string.IsNullOrEmpty(qSearch.ChannelT) || 
                        v.ChannelTitle?.Contains(qSearch.ChannelT, StringComparison.OrdinalIgnoreCase) == true) &&
                        (string.IsNullOrEmpty(qSearch.Descrip) || 
                        v.ChannelDescription?.Contains(qSearch.Descrip, StringComparison.OrdinalIgnoreCase) == true)
                    )
                    .ToList();

                if (filteredVideos != null && filteredVideos.Count() > 0)
                {
                    return filteredVideos;
                }

                else throw new ArgumentException("NÃO FORAM ENCONTRADOS VÍDEOS COM OS FILTROS ESPECIFICADOS!");
            }

            else throw new ArgumentException("UM ERRO INESPERADO ACONTECEU. TENTE NOVAMENTE MAIS TARDE!");
        }

        public async Task<string> SaveAllVideos(YTBVideoSearchDTO allVideos, string apiKey)
        {
            if(allVideos == null || allVideos.Items == null) throw new ArgumentException("NÃO EXISTEM VÍDEOS A SEREM SALVOS!");

            foreach (var video in allVideos.Items) 
            {
                var exists = await _db.YTBVideoSearches
                    .AnyAsync(v => v.VideoId == video.Id.VideoId);

                if (exists) continue;

                var videoDuration = await GetYouTubeVideoDuration(video.Id.VideoId, apiKey);

                var saveVideo = new YTBVideoSearch
                {
                    VideoId = video?.Id?.VideoId ?? string.Empty,
                    VideoTitle = video?.Snippet?.Title ?? string.Empty,
                    ChannelTitle = video?.Snippet?.ChannelTitle ?? string.Empty,
                    ChannelDescription = video?.Snippet?.Description ?? string.Empty,
                    Duration = videoDuration ?? string.Empty,
                    Date = video?.Snippet?.PublishedAt,
                    Thumbnails = new List<ThumbnailItem>
                 {
                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.Default,
                         Url = video?.Snippet?.Thumbnails?.Default?.Url ?? string.Empty,
                         Width = video?.Snippet?.Thumbnails?.Default?.Width ?? 0,
                         Height = video?.Snippet?.Thumbnails?.Default?.Height ?? 0
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.Medium,
                         Url = video?.Snippet?.Thumbnails?.Medium?.Url ?? string.Empty,
                         Width = video?.Snippet?.Thumbnails?.Medium?.Width ?? 0,
                         Height = video?.Snippet?.Thumbnails?.Medium?.Height ?? 0
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.High,
                         Url = video?.Snippet?.Thumbnails?.High?.Url ?? string.Empty,
                         Width = video?.Snippet?.Thumbnails?.High?.Width ?? 0,
                         Height = video?.Snippet?.Thumbnails?.High?.Height ?? 0
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.Standard,
                         Url = video?.Snippet?.Thumbnails?.Standard?.Url ?? string.Empty,
                         Width = video?.Snippet?.Thumbnails?.Standard?.Width ?? 0,
                         Height = video?.Snippet?.Thumbnails?.Standard?.Height ?? 0
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.Maxres,
                         Url = video?.Snippet?.Thumbnails?.Maxres?.Url ?? string.Empty,
                         Width = video?.Snippet?.Thumbnails?.Maxres?.Width ?? 0,
                         Height = video?.Snippet?.Thumbnails?.Maxres?.Height ?? 0
                     }
                 },
                    IsDeleted = false
                };

                await _db.YTBVideoSearches.AddAsync(saveVideo);
                await _db.SaveChangesAsync();
            }

            return "TODOS OS VÍDEOS FORAM SALVOS COM SUCESSO!";
        }

        public async Task<string> EditVideo(int id, VideoEditFields field, string data)
        {
            var video = await GetVideoById(id);

            if(video == null) 
            {
                throw new ArgumentException("VÍDEO NÃO ENCONTRADO NO BANCO DE DADOS!");
            }

            if(field == VideoEditFields.VideoTitle)
            {
                video.VideoTitle = data;
            }

            else if (field == VideoEditFields.ChannelTitle)
            {
                video.ChannelTitle = data;
            }

            else if (field == VideoEditFields.ChannelDescription)
            {
                video.ChannelDescription = data;
            }

            _db.YTBVideoSearches.Update(video);
            await _db.SaveChangesAsync();

            return ("INFORMAÇÕES DO VÍDEO ATUALIZADAS!");
        }

        public async Task<string> DeleteVideo(int id) 
        {
            var video = await GetVideoById(id);

            if (video == null)
            {
                throw new ArgumentException("VÍDEO NÃO ENCONTRADO NO BANCO DE DADOS!");
            }

            video.IsDeleted = true;

            _db.YTBVideoSearches.Update(video);
            await _db.SaveChangesAsync();

            return ("VÍDEO DELETADO COM SUCESSO!");
        }

        public void DeleteAllVideos()
        {
            var allVideos = _db.YTBVideoSearches.ToList();
            _db.YTBVideoSearches.RemoveRange(allVideos);
            _db.SaveChanges();
        }
    }
}
