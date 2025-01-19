using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using YouTubei9.Services.VideoAPI.Data;
using YouTubei9.Services.VideoAPI.Functions;
using YouTubei9.Services.VideoAPI.Models;
using YouTubei9.Services.VideoAPI.Models.DTO;
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
        [Route("SaveVideo")]
        public ActionResult SaveVideo(string videoId)
        {

            try
            {
                try
                {
                    videoSearch.ValidateData();
                }

                catch (ArgumentException ex)
                {
                    return StatusCode(400, ex.Message);
                }

                var result = videoSearch.SaveVideo(videoId);

                return Ok(result);
            }

            catch (ArgumentException ex) 
            {
                return StatusCode(404, ex.Message);
            }
        }

        [HttpGet]
        [Route("EditVideos")]

        [HttpGet]
        [Route("DeleteVideos")]


        [HttpGet]
        [Route("ListVideos")]
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
        [Route("GetVideosFiltered/{filter}/{search}")]
        public ActionResult<string> GetVideosByFilter(VideoFilters filter, string search)
        {
            var videosList = new List<ResponseItem>();

            try
            {
                try
                {
                    videoSearch.ValidateData();
                }

                catch (ArgumentException ex) 
                {
                    return StatusCode(400, ex.Message);
                }

                videosList = videoSearch.GetVideosByFilter(filter, search);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonVideos = JsonSerializer.Serialize(videosList, options);

                return jsonVideos;
            }

            catch (ArgumentException ex) 
            {
                return StatusCode(400, ex.Message);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "ERRO INTERNO DO SERVIDOR!" });
            }

        }
    }
}
