using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubei9.Services.VideoAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddYTBVideoSearchesToDb3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThumbnailItem_YTBVideoSearches_YTBVideoSearchId",
                table: "ThumbnailItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThumbnailItem",
                table: "ThumbnailItem");

            migrationBuilder.DropColumn(
                name: "ThumbType",
                table: "YTBVideoSearches");

            migrationBuilder.RenameTable(
                name: "ThumbnailItem",
                newName: "YTBVideoSearchesThumbs");

            migrationBuilder.RenameIndex(
                name: "IX_ThumbnailItem_YTBVideoSearchId",
                table: "YTBVideoSearchesThumbs",
                newName: "IX_YTBVideoSearchesThumbs_YTBVideoSearchId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "YTBVideoSearches",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ThumbType",
                table: "YTBVideoSearchesThumbs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_YTBVideoSearchesThumbs",
                table: "YTBVideoSearchesThumbs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_YTBVideoSearchesThumbs_YTBVideoSearches_YTBVideoSearchId",
                table: "YTBVideoSearchesThumbs",
                column: "YTBVideoSearchId",
                principalTable: "YTBVideoSearches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YTBVideoSearchesThumbs_YTBVideoSearches_YTBVideoSearchId",
                table: "YTBVideoSearchesThumbs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_YTBVideoSearchesThumbs",
                table: "YTBVideoSearchesThumbs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "YTBVideoSearches");

            migrationBuilder.DropColumn(
                name: "ThumbType",
                table: "YTBVideoSearchesThumbs");

            migrationBuilder.RenameTable(
                name: "YTBVideoSearchesThumbs",
                newName: "ThumbnailItem");

            migrationBuilder.RenameIndex(
                name: "IX_YTBVideoSearchesThumbs_YTBVideoSearchId",
                table: "ThumbnailItem",
                newName: "IX_ThumbnailItem_YTBVideoSearchId");

            migrationBuilder.AddColumn<int>(
                name: "ThumbType",
                table: "YTBVideoSearches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThumbnailItem",
                table: "ThumbnailItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThumbnailItem_YTBVideoSearches_YTBVideoSearchId",
                table: "ThumbnailItem",
                column: "YTBVideoSearchId",
                principalTable: "YTBVideoSearches",
                principalColumn: "Id");
        }
    }
}
