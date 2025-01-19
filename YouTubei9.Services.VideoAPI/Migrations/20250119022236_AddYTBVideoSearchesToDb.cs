using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubei9.Services.VideoAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddYTBVideoSearchesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YTBVideoSearches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YTBVideoSearches", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YTBVideoSearches");
        }
    }
}
