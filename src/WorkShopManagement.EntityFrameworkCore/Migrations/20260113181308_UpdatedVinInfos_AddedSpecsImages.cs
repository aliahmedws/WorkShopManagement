using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedVinInfos_AddedSpecsImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ImagesLastUpdated",
                table: "AppVinInfos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagesResponse",
                table: "AppVinInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SpecsLastUpdated",
                table: "AppVinInfos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecsResponse",
                table: "AppVinInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagesLastUpdated",
                table: "AppVinInfos");

            migrationBuilder.DropColumn(
                name: "ImagesResponse",
                table: "AppVinInfos");

            migrationBuilder.DropColumn(
                name: "SpecsLastUpdated",
                table: "AppVinInfos");

            migrationBuilder.DropColumn(
                name: "SpecsResponse",
                table: "AppVinInfos");
        }
    }
}
