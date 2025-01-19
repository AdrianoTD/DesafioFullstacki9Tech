using YouTubei9.Services.VideoAPI.Models.VideoSearchComponents;

namespace YouTubei9.Services.VideoAPI.Models.DTO
{
    public class YTBVideoSearchDTO
    {
        public string? Kind { get; set; }
        public string? Etag { get; set; }
        public string? NextPageToken { get; set; }
        public string? RegionCode { get; set; }
        public PageInfo? PageInfo { get; set; }
        public List<ResponseItem>? Items { get; set; }
    }

    public class PageInfo
    {
        public int TotalResults { get; set; }
        public int ResultsPerPage { get; set; }
    }
}

