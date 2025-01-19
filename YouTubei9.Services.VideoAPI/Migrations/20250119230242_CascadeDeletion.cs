using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubei9.Services.VideoAPI.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YTBVideoSearchesThumbs_YTBVideoSearches_YTBVideoSearchId",
                table: "YTBVideoSearchesThumbs");

            migrationBuilder.AlterColumn<int>(
                name: "YTBVideoSearchId",
                table: "YTBVideoSearchesThumbs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_YTBVideoSearchesThumbs_YTBVideoSearches_YTBVideoSearchId",
                table: "YTBVideoSearchesThumbs",
                column: "YTBVideoSearchId",
                principalTable: "YTBVideoSearches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YTBVideoSearchesThumbs_YTBVideoSearches_YTBVideoSearchId",
                table: "YTBVideoSearchesThumbs");

            migrationBuilder.AlterColumn<int>(
                name: "YTBVideoSearchId",
                table: "YTBVideoSearchesThumbs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_YTBVideoSearchesThumbs_YTBVideoSearches_YTBVideoSearchId",
                table: "YTBVideoSearchesThumbs",
                column: "YTBVideoSearchId",
                principalTable: "YTBVideoSearches",
                principalColumn: "Id");
        }
    }
}
