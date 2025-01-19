namespace YouTubei9.Services.VideoAPI.Models.VideoSearchComponents
{
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

    public class ThumbnailItem
    {
        public int Id { get; set; }
        public ThumbnailType ThumbType { get; set; }
        public string? Url { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }

    public enum ThumbnailType
    {
        Default,
        Medium,
        High,
        Standard,
        Maxres
    }
}
