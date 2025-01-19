using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.Json;
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
                         Url = video.Snippet.Thumbnails.Default.Url,
                         Width = video.Snippet.Thumbnails.Default.Width,
                         Height = video.Snippet.Thumbnails.Default.Height
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.Medium,
                         Url = video.Snippet.Thumbnails.Medium.Url,
                         Width = video.Snippet.Thumbnails.Medium.Width,
                         Height = video.Snippet.Thumbnails.Medium.Height
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.High,
                         Url = video.Snippet.Thumbnails.High.Url,
                         Width = video.Snippet.Thumbnails.High.Width,
                         Height = video.Snippet.Thumbnails.High.Height
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.Standard,
                         Url = video.Snippet.Thumbnails.Standard.Url,
                         Width = video.Snippet.Thumbnails.Standard.Width,
                         Height = video.Snippet.Thumbnails.Standard.Height
                     },

                     new ThumbnailItem
                     {
                         ThumbType = ThumbnailType.Maxres,
                         Url = video.Snippet.Thumbnails.Maxres.Url,
                         Width = video.Snippet.Thumbnails.Maxres.Width,
                         Height = video.Snippet.Thumbnails.Maxres.Height
                     }
                 },
                 IsDeleted = false
             };

            ;
            //INSERIR NO BANCO E SER FELIZ

            return "VÍDEO INSERIDO COM SUCESSO!";
        }
    }
}
