using YouTubei9.Services.VideoAPI.Models.VideoSearchComponents;
using System.ComponentModel.DataAnnotations;

namespace YouTubei9.Services.VideoAPI.Models
{
    public class YTBVideoSearch
    {
        public int Id { get; set; }
        public string? VideoId { get; set; }
        public string? ChannelTitle { get; set; }
        public List<ThumbnailItem>? Thumbnails { get; set; }
        public bool IsDeleted { get; set; }
    }

    
}
