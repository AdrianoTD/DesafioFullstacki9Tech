namespace YouTubei9.Services.VideoAPI.Models
{
    public class YTBVideoSearchResponse
    {
        public string? Kind = "youtube#searchResult";
        public string? Etag { get; set; }
        public Id? Id { get; set; }
        public Snippet? Snippet { get; set; }
        public string? ChannelTitle { get; set; }
        public string? LiveBroadcastContent { get; set; }
    }

    public class Id
    {
        public string? Kind { get; set; }
        public string? VideoId { get; set; }
        public string? ChannelId { get; set; }
        public string? PlaylistId { get; set; }

    }

    public class Snippet
    {
        public DateTime? PublishedAt { get; set; }
        public string? ChannelId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }

    public class Thumbnails
    {
        public Thumbnail? Default { get; set; }
        public Thumbnail? Medium { get; set; }
        public Thumbnail? High { get; set; }
        public Thumbnail? Standard { get; set; }
        public Thumbnail? Maxres { get; set; }
    }

    public class Thumbnail
    {
        public string? Url { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}
