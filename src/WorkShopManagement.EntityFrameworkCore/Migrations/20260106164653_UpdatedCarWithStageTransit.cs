using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCarWithStageTransit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookingNumber",
                table: "AppCars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClearingAgent",
                table: "AppCars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EtaBrisbane",
                table: "AppCars",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EtaScd",
                table: "AppCars",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationStatus",
                table: "AppCars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "AppCars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StorageLocation",
                table: "AppCars",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingNumber",
                table: "AppCars");

            migrationBuilder.DropColumn(
                name: "ClearingAgent",
                table: "AppCars");

            migrationBuilder.DropColumn(
                name: "EtaBrisbane",
                table: "AppCars");

            migrationBuilder.DropColumn(
                name: "EtaScd",
                table: "AppCars");

            migrationBuilder.DropColumn(
                name: "LocationStatus",
                table: "AppCars");

            migrationBuilder.DropColumn(
                name: "Stage",
                table: "AppCars");

            migrationBuilder.DropColumn(
                name: "StorageLocation",
                table: "AppCars");
        }
    }
}
