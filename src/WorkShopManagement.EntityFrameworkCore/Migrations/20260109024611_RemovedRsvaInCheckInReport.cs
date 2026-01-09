using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRsvaInCheckInReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppCheckInReports_CarId",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "RsvaImportApproval",
                table: "AppCheckInReports");

            migrationBuilder.CreateIndex(
                name: "IX_AppCheckInReports_CarId",
                table: "AppCheckInReports",
                column: "CarId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppCheckInReports_CarId",
                table: "AppCheckInReports");

            migrationBuilder.AddColumn<string>(
                name: "RsvaImportApproval",
                table: "AppCheckInReports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCheckInReports_CarId",
                table: "AppCheckInReports",
                column: "CarId");
        }
    }
}
