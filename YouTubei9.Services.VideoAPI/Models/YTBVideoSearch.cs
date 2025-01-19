using YouTubei9.Services.VideoAPI.Models.VideoSearchComponents;
using System.ComponentModel.DataAnnotations;
using YouTubei9.Services.VideoAPI.Data;

namespace YouTubei9.Services.VideoAPI.Models
{
    public class YTBVideoSearch
    {
        public int Id { get; set; }
        public string VideoId { get; set; } = string.Empty;
        public string VideoTitle { get; set; } = string.Empty;
        public string ChannelTitle { get; set; } = string.Empty;
        public string ChannelDescription { get; set; } = string.Empty;
        public string Duration {  get; set; } = string.Empty;
        public DateTime? Date {  get; set; }
        public List<ThumbnailItem>? Thumbnails { get; set; }
        public bool IsDeleted { get; set; }
    };

    public enum VideoFilters
    {
        Title,
        Duration,
        Author,
        ByDate,
        Q //Seguindo o padrão: (TITULO, NOME_CANAL, DESCRICAO)
    }

    public enum VideoEditFields
    {
        VideoTitle,
        ChannelTitle,
        ChannelDescription
    }
}
