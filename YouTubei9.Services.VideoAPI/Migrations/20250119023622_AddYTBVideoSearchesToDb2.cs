using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubei9.Services.VideoAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddYTBVideoSearchesToDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelTitle",
                table: "YTBVideoSearches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThumbType",
                table: "YTBVideoSearches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VideoId",
                table: "YTBVideoSearches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ThumbnailItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YTBVideoSearchId = table.Column<int>(type: "int", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThumbnailItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThumbnailItem_YTBVideoSearches_YTBVideoSearchId",
                        column: x => x.YTBVideoSearchId,
                        principalTable: "YTBVideoSearches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThumbnailItem_YTBVideoSearchId",
                table: "ThumbnailItem",
                column: "YTBVideoSearchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThumbnailItem");

            migrationBuilder.DropColumn(
                name: "ChannelTitle",
                table: "YTBVideoSearches");

            migrationBuilder.DropColumn(
                name: "ThumbType",
                table: "YTBVideoSearches");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "YTBVideoSearches");
        }
    }
}
