namespace YouTubei9.Services.VideoAPI.Models.VideoSearchComponents
{
    public class Snippet
    {
        public DateTime? PublishedAt { get; set; }
        public string? ChannelId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Thumbnails? Thumbnails { get; set; }
        public string? ChannelTitle { get; set; }
        public string? LiveBroadcastContent { get; set; }
        public DateTime? PublishTime { get; set; }
    }
}
