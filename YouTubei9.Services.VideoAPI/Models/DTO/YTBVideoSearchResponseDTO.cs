using YouTubei9.Services.VideoAPI.Models.VideoSearchComponents;

namespace YouTubei9.Services.VideoAPI.Models.DTO
{
    public class YTBVideoSearchResponseDTO
    {
        public string? Kind = "youtube#searchResult";
        public string? Etag { get; set; }
        public Id? Id { get; set; }
        public Snippet? Snippet { get; set; }
        public string? ChannelTitle { get; set; }
        public string? LiveBroadcastContent { get; set; }
    }
}
