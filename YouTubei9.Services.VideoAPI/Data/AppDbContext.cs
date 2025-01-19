using Microsoft.EntityFrameworkCore;
using YouTubei9.Services.VideoAPI.Models;

namespace YouTubei9.Services.VideoAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<YTBVideoSearchResponse> YTBVideoSearches { get; set; }
    }
}
