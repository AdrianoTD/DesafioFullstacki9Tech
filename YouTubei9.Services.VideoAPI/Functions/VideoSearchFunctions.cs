using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using YouTubei9.Services.VideoAPI.Data;
using YouTubei9.Services.VideoAPI.Models;
using YouTubei9.Services.VideoAPI.Models.DTO;
using YouTubei9.Services.VideoAPI.Models.VideoSearchComponents;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YouTubei9.Services.VideoAPI.Functions
{
    public class VideoSearchFunctions
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private static YTBVideoSearchDTO _videos = new YTBVideoSearchDTO();

        public VideoSearchFunctions(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<string> SearchYoutubeVideos()
        {
            var apiKey = _configuration["YTB_API_KEY"];

            var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q=dotnet8&type=video&relevanceLanguage=pt&publishedAfter=2025-01-01T00:00:00Z&maxResults=15&key={apiKey}";

            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var videos = JsonConvert.DeserializeObject<YTBVideoSearchDTO>(responseBody);

                if(videos != null) _videos = videos;


                return responseBody;
                //Console.WriteLine($"Resposta da API: {responseBody}");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("ERRO AO SE COMUNICAR COM A API: ", ex.Message);
            }
        }

        public List<YTBVideoSearch> GetVideos()
        {
            List<YTBVideoSearch> videosList = _db.YTBVideoSearches.ToList();
            List<ThumbnailItem> thumbsList = _db.YTBVideoSearchesThumbs.ToList();

            if(videosList == null || videosList.Count == 0 || thumbsList == null || thumbsList.Count == 0)
            {
                throw new ArgumentException("NÃO FORAM ENCONTRADOS VÍDEOS NO SEU BANCO DE DADOS!");
            }

            return videosList;
        }

        public YTBVideoSearch GetVideoById(int id)
        {
            var video = _db.YTBVideoSearches.FirstOrDefault(v => v.Id == id);

            if(video == null)
            {
                throw new ArgumentException("VÍDEO NÃO ENCONTRADO!");
            }

            return video;
        }

        public void ValidateData()
        {
            if (_videos == null || _videos.Items == null || _videos.Items.Count == 0)
            {
                throw new ArgumentException("REALIZE UMA BUSCA PRIMEIRO!");
            }

            else return;
        }

        public List<ResponseItem> GetVideosByFilter(VideoFilters filter, string search)
        {
            var filteredVideos = new List<ResponseItem>();
            search = search.ToLower();

            if (filter == VideoFilters.Title)
            {
                filteredVideos = _videos.Items
                    .Where(v => v.Snippet?.Title != null && v.Snippet.Title.ToLower().Contains(search))
                    .ToList();

                if (filteredVideos != null && filteredVideos.Count() > 0)
                {
                    return filteredVideos;
                }

                else throw new ArgumentException("NÃO FORAM ENCONTRADOS VÍDEOS COM ESTE TÍTULO!");
            }

            /*else if (filter == VideoFilters.Duration) 
            {
                videos = _db.YTBVideoSearches.Where(u => u.Duration <= search).ToList();
            }*/

            else if (filter == VideoFilters.Author)
            {
                filteredVideos = _videos.Items
                    .Where(u => u.Snippet?.ChannelTitle.ToLower() == search).ToList();

                if (filteredVideos != null && filteredVideos.Count() > 0)
                {
                    return filteredVideos;
                }

                else throw new ArgumentException("NÃO FORAM ENCONTRADOS VÍDEOS DO AUTOR ESPECIFICADO!");
            }

            else if (filter == VideoFilters.ByDate)
            {
                string format = "dd-MM-yyyy";

                if (DateTime.TryParseExact(search, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    if (parsedDate > DateTime.Now) 
                    {
                        throw new ArgumentException("A DATA ULTRAPASSA O PERÍODO VÁLIDO!");
                    }
                    filteredVideos = _videos.Items
                    .Where(v => v.Snippet?.PublishedAt >= parsedDate && parsedDate.Year == 2025)
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

                if(terms.Length < 1 || terms.Length > 3)
                {
                    throw new ArgumentException("QUANTIDADE INVÁLIDA DE TERMOS!");
                }

                var qSearch = (
                    Title: terms.ElementAtOrDefault(0), 
                    ChannelT: terms.ElementAtOrDefault(1) ?? string.Empty, 
                    Descrip: terms.ElementAtOrDefault(2) ?? string.Empty
                    );

                filteredVideos = _videos.Items
                    .Where(v =>
                        (string.IsNullOrEmpty(qSearch.Title) || v.Snippet?.Title.Contains(qSearch.Title, StringComparison.OrdinalIgnoreCase) == true) &&
                        (string.IsNullOrEmpty(qSearch.ChannelT) || v.Snippet?.ChannelTitle.Contains(qSearch.ChannelT, StringComparison.OrdinalIgnoreCase) == true) &&
                        (string.IsNullOrEmpty(qSearch.Descrip) || v.Snippet?.Description.Contains(qSearch.Descrip, StringComparison.OrdinalIgnoreCase) == true)
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

         public string SaveVideo(string videoId)
         {
             var video = _videos.Items
                 .FirstOrDefault(v => v.Id?.VideoId == videoId);

            if (video == null) 
            {
                throw new ArgumentException("NÃO FOI ENCONTRADO UM VÍDEO COM ESTE ID!");
            }

            

             var saveVideo = new YTBVideoSearch
             {
                 VideoId = video.Id.VideoId,
                 VideoTitle = video.Snippet.Title,
                 ChannelTitle = video.Snippet.ChannelTitle,
                 ChannelDescription = video.Snippet.Description,
                 Duration = string.Empty,
                 Date = video.Snippet?.PublishedAt,
                 Thumbnails = new List<ThumbnailItem>
                 {
                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.Default,
                         Url = video.Snippet.Thumbnails?.Default?.Url ?? string.Empty,
                         Width = video.Snippet.Thumbnails?.Default?.Width ?? 0,
                         Height = video.Snippet.Thumbnails?.Default?.Height ?? 0
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.Medium,
                         Url = video.Snippet.Thumbnails?.Medium?.Url ?? string.Empty,
                         Width = video.Snippet.Thumbnails?.Medium?.Width ?? 0,
                         Height = video.Snippet.Thumbnails?.Medium?.Height ?? 0
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.High,
                         Url = video.Snippet.Thumbnails?.High?.Url ?? string.Empty,
                         Width = video.Snippet.Thumbnails?.High?.Width ?? 0,
                         Height = video.Snippet.Thumbnails?.High?.Height ?? 0
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.Standard,
                         Url = video.Snippet.Thumbnails?.Standard?.Url ?? string.Empty,
                         Width = video.Snippet.Thumbnails?.Standard?.Width ?? 0,
                         Height = video.Snippet.Thumbnails?.Standard?.Height ?? 0
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.Maxres,
                         Url = video.Snippet.Thumbnails?.Maxres?.Url ?? string.Empty,
                         Width = video.Snippet.Thumbnails?.Maxres?.Width ?? 0,
                         Height = video.Snippet.Thumbnails?.Maxres?.Height ?? 0
                     }
                 },
                 IsDeleted = false
             };
            
            _db.YTBVideoSearches.Add(saveVideo);
            _db.SaveChanges();

            return "VÍDEO INSERIDO COM SUCESSO!";
        }

        public string EditVideo(int id, VideoEditFields field, string data)
        {
            var video = GetVideoById(id);

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
            _db.SaveChanges();

            return ("INFORMAÇÕES DO VÍDEO ATUALIZADAS!");
        }

        public string DeleteVideo(int id) 
        {
            var video = GetVideoById(id);

            if (video == null)
            {
                throw new ArgumentException("VÍDEO NÃO ENCONTRADO NO BANCO DE DADOS!");
            }

            video.IsDeleted = true;

            _db.YTBVideoSearches.Update(video);
            _db.SaveChanges();

            return ("VÍDEO DELETADO COM SUCESSO!");
        }
    }
}
