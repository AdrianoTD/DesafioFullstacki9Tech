using Microsoft.AspNetCore.Authorization;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        [Route("YouTubeAPI/SaveAllDotNet8Videos")]
        public async Task<ActionResult<string>> SaveAllYouTubeVideos()
        {
            var apiKey = HttpContext.Items["ApiKey"]?.ToString();
            try
            {
                var response = await videoSearch.SaveAllYoutubeVideos(apiKey);

                return Ok(response);
            }

            catch (ArgumentException ex)
            {
                return StatusCode(404, ex.Message);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("YouTubeAPI/GetYTBVideoDuration")]
        public async Task<ActionResult<YTBVideoInfoDTO>> GetYouTubeVideoDuration(string videoId)
        {
            var apiKey = HttpContext.Items["ApiKey"]?.ToString();
            try
            {
                var response = await videoSearch.GetYouTubeVideoDuration(videoId, apiKey);

                return Ok(response);
            }

            catch (ArgumentException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("ListVideos")]
        public ActionResult<List<YTBVideoSearch>> GetVideos()
        {
            try
            {
                var videosList = videoSearch.GetVideos();

                return videosList;
            }

            catch (ArgumentException ex)
            {
                return StatusCode(404, ex.Message);
            }

            catch (Exception ex)
            {
                return StatusCode(500, "OCORREU UM ERRO AO BUSCAR SEUS VÍDEOS NO BANCO DE DADOS!");
            }
        }


        [HttpGet]
        [Route("GetVideosFilteredFromDatabase/{filter}/{search}")]
        public async Task<ActionResult<string>> GetVideosByFilterFromDatabase(VideoFilters filter, string search)
        {
            var videosList = new List<YTBVideoSearch>();

            try
            {
                videosList = videoSearch.GetVideosByFilterFromDatabase(filter, search);

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

        [HttpGet]
        [Route("EditVideo/{id}")]
        public async Task<ActionResult<string>> EditVideo(int id, VideoEditFields field, string data)
        {
            try
            {
                var response = await videoSearch.EditVideo(id, field, data);

                return response;
            }

            catch (ArgumentException ex)
            {
                return StatusCode(404, ex.Message);
            }

            catch (Exception ex)
            {
                return StatusCode(500, "OCORREU UM ERRO INESPERADO E NÃO FOI POSSÍVEL EDITAR O VÍDEO!");
            }
        }

        [HttpGet]
        [Route("DeleteVideo")]
        public async Task<ActionResult<string>> DeleteVideo(int id)
        {
            try
            {
                var response = await videoSearch.DeleteVideo(id);

                return response;
            }

            catch (ArgumentException ex)
            {
                return StatusCode(404, ex.Message);
            }

            catch (Exception ex)
            {
                return StatusCode(500, "OCORREU UM ERRO INESPERADO E NÃO FOI POSSÍVEL EDITAR O VÍDEO!");
            }
        }

        [HttpGet]
        [Route("DeleteAllVideos(For tests only)")]
        public async Task<string> DeleteAllVideos()
        {
            videoSearch.DeleteAllVideos();

            return "TODOS OS VÍDEOS DELETADOS";
        }
    }
}
