using Microsoft.EntityFrameworkCore;
using YouTubei9.Services.VideoAPI.Models;
using YouTubei9.Services.VideoAPI.Models.VideoSearchComponents;

namespace YouTubei9.Services.VideoAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<YTBVideoSearch> YTBVideoSearches { get; set; }
        public DbSet<ThumbnailItem> YTBVideoSearchesThumbs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração explícita da chave primária (opcional se o atributo [Key] já foi usado)
            modelBuilder.Entity<YTBVideoSearch>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<ThumbnailItem>()
                .HasKey(e => e.Id);
        }
    }
}
