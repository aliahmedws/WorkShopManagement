using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class add_CarBayId_QualityGates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCarBays_AppQualityGates_QualityGateId",
                table: "AppCarBays");

            migrationBuilder.DropIndex(
                name: "IX_AppCarBays_QualityGateId",
                table: "AppCarBays");

            migrationBuilder.DropColumn(
                name: "QualityGateId",
                table: "AppCarBays");

            migrationBuilder.AddColumn<Guid>(
                name: "CarBayId",
                table: "AppQualityGates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppQualityGates_CarBayId",
                table: "AppQualityGates",
                column: "CarBayId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppQualityGates_AppCarBays_CarBayId",
                table: "AppQualityGates",
                column: "CarBayId",
                principalTable: "AppCarBays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppQualityGates_AppCarBays_CarBayId",
                table: "AppQualityGates");

            migrationBuilder.DropIndex(
                name: "IX_AppQualityGates_CarBayId",
                table: "AppQualityGates");

            migrationBuilder.DropColumn(
                name: "CarBayId",
                table: "AppQualityGates");

            migrationBuilder.AddColumn<Guid>(
                name: "QualityGateId",
                table: "AppCarBays",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCarBays_QualityGateId",
                table: "AppCarBays",
                column: "QualityGateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCarBays_AppQualityGates_QualityGateId",
                table: "AppCarBays",
                column: "QualityGateId",
                principalTable: "AppQualityGates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
