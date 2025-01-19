using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using YouTubei9.Services.VideoAPI.Data;
using YouTubei9.Services.VideoAPI.Functions;
using YouTubei9.Services.VideoAPI.Models;
using YouTubei9.Services.VideoAPI.Models.VideoSearchComponents;

namespace YouTubei9.Services.VideoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YTBSearchController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        public VideoSearchFunctions videoSearch;

        public YTBSearchController(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;

            videoSearch = new VideoSearchFunctions(_db, configuration);
        }

        [HttpGet]
        [Route("YouTubeAPI/GetDotNet8Videos")]
        public async Task<ActionResult<string>> SearchYouTubeVideos() 
        {
            try
            {
                var response = await videoSearch.SearchYoutubeVideos();
                return Ok(response);
            }

            catch (ArgumentException ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public List<YTBVideoSearch> GetVideos() 
        {
            try
            {
                List<YTBVideoSearch> videosList = _db.YTBVideoSearches.ToList();
                return videosList;
            }

            catch (Exception ex) { }

            return null;
        }

        [HttpGet]
        [Route("DataSearch/{filter}/{search}")]
        public ActionResult<List<YTBVideoSearch>> GetVideosByFilter(VideoFilters filter, string search)
        {
            var videosList = new List<YTBVideoSearch>();

            try
            {
                videosList = videoSearch.GetVideosByFilter(filter, search);

                return videosList;
            }

            catch (ArgumentException ex) 
            {
                return StatusCode(404, ex.Message);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "ERRO INTERNO DO SERVIDOR!" });
            }

        }

        /*[HttpGet]
        public List<ThumbnailItem> GetThumbs()
        {
            try
            {
                List<ThumbnailItem> thumbsList = _db.YTBVideoSearchesThumbs.ToList();
                return thumbsList;
            }

            catch (Exception ex) { }

            return null;
        }*/
    }
}
