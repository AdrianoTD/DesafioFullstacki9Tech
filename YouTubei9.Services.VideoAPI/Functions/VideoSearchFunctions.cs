using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using YouTubei9.Services.VideoAPI.Data;
using YouTubei9.Services.VideoAPI.Models;

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

        public void PrintSettings()
        {
            var apiUrl = _configuration["MySettings:ApiUrl"];
            var apiKey = _configuration["MySettings:ApiKey"];

            Console.WriteLine($"API Url: {apiUrl}");
            Console.WriteLine($"API Key: {apiKey}");
        }

        public async Task<string> SearchYoutubeVideos()
        {
            var apiKey = _configuration["YTB_API_KEY"];

            var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q=dotnet8&type=video&relevanceLanguage=pt&key={apiKey}";

            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
                //Console.WriteLine($"Resposta da API: {responseBody}");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("ERRO AO SE COMUNICAR COM A API: ", ex.Message);
            }
        }

        public List<YTBVideoSearch> GetVideosByFilter(VideoFilters filter, string search)
        {
            var videos = new List<YTBVideoSearch>();
            search = search.ToLower();

            if (filter == VideoFilters.Title)
            {
                YTBVideoSearch video = _db.YTBVideoSearches.FirstOrDefault(u => u.VideoTitle.ToLower() == search);

                if (video != null)
                {
                    videos.Add(video);
                    return videos;
                }

                else
                {
                    throw new ArgumentException("VÍDEO NÃO ENCONTRADO!");
                }
            }

            /*else if (filter == VideoFilters.Duration) 
            {
                videos = _db.YTBVideoSearches.Where(u => u.Duration <= search).ToList();
            }*/

            else if (filter == VideoFilters.Author)
            {
                videos = _db.YTBVideoSearches.Where(u => u.Author.ToLower() == search).ToList();

                if (videos != null && videos.Count() > 0)
                {
                    return videos;
                }

                else
                {
                    throw new ArgumentException("NÃO FORAM ENCONTRADOS VÍDEOS DO AUTOR ESPECIFICADO!");
                }
            }

            else if (filter == VideoFilters.ByDate)
            {
                string format = "dd-MM-yyyy";

                DateTime date = DateTime.ParseExact(search, format, System.Globalization.CultureInfo.InvariantCulture);

                videos = _db.YTBVideoSearches.Where(u => u.Date <= date).ToList();

                if (videos != null && videos.Count() > 0)
                {
                    return videos;
                }

                else
                {
                    throw new ArgumentException("NÃO FORAM ENCONTRADOS VÍDEOS NO PERÍODO ESPECIFICADO!");
                }
            }

            else throw new ArgumentException("UM ERRO INESPERADO ACONTECEU. TENTE NOVAMENTE MAIS TARDE!"); 
        }
    }
}
