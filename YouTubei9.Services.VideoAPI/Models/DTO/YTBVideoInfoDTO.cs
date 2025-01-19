using YouTubei9.Services.VideoAPI.Models.VideoInfoComponents;
using YouTubei9.Services.VideoAPI.Models.VideoSearchComponents;

namespace YouTubei9.Services.VideoAPI.Models.DTO
{
    public class YTBVideoInfoDTO
    {
        public string? Kind { get; set; }
        public string? Etag { get; set; }
        public List<VideoInfo>? Items { get; set; }
    }
}
