using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddVinInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppVinInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VinNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VinResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecallResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VinLastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecallLastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVinInfos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppVinInfos_VinNo",
                table: "AppVinInfos",
                column: "VinNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppVinInfos");
        }
    }
}
