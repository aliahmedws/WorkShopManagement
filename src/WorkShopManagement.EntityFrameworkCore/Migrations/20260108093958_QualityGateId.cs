using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class QualityGateId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
