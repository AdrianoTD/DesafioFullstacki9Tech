using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubei9.Services.VideoAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddYTBVideoSearchesToDb6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "YTBVideoSearches");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "YTBVideoSearches",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "YTBVideoSearches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "YTBVideoSearches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
