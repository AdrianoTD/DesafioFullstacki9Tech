namespace YouTubei9.Services.VideoAPI.Models.VideoInfoComponents
{
    public class ContentDetail
    {
        public string? Duration { get; set; }
        public string? Dimension { get; set; }
        public string? Definition { get; set; }
        public string? Caption { get; set; }
        public bool LicensedContent { get; set; }
        public Restriction? RegionRestriction { get; set; }
    }

    public class Restriction
    {
        public List<string>? Blocked { get; set; }
    }
}
