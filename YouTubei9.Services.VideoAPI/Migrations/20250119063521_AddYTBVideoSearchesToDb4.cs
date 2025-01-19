using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubei9.Services.VideoAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddYTBVideoSearchesToDb4 : Migration
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

            migrationBuilder.AlterColumn<string>(
                name: "VideoId",
                table: "YTBVideoSearches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChannelTitle",
                table: "YTBVideoSearches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "YTBVideoSearches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChannelDescription",
                table: "YTBVideoSearches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "YTBVideoSearches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "YTBVideoSearches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VideoTitle",
                table: "YTBVideoSearches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.DropColumn(
                name: "Author",
                table: "YTBVideoSearches");

            migrationBuilder.DropColumn(
                name: "ChannelDescription",
                table: "YTBVideoSearches");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "YTBVideoSearches");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "YTBVideoSearches");

            migrationBuilder.DropColumn(
                name: "VideoTitle",
                table: "YTBVideoSearches");

            migrationBuilder.AlterColumn<int>(
                name: "YTBVideoSearchId",
                table: "YTBVideoSearchesThumbs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "VideoId",
                table: "YTBVideoSearches",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ChannelTitle",
                table: "YTBVideoSearches",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_YTBVideoSearchesThumbs_YTBVideoSearches_YTBVideoSearchId",
                table: "YTBVideoSearchesThumbs",
                column: "YTBVideoSearchId",
                principalTable: "YTBVideoSearches",
                principalColumn: "Id");
        }
    }
}
