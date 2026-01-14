using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class add_ClockIn_properties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClockInStatus",
                table: "AppCarBays",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClockInTime",
                table: "AppCarBays",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClockOutTime",
                table: "AppCarBays",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClockInStatus",
                table: "AppCarBays");

            migrationBuilder.DropColumn(
                name: "ClockInTime",
                table: "AppCarBays");

            migrationBuilder.DropColumn(
                name: "ClockOutTime",
                table: "AppCarBays");
        }
    }
}
