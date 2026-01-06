using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueFromPriorityNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppPriorities_Number",
                table: "AppPriorities");

            migrationBuilder.CreateIndex(
                name: "IX_AppPriorities_Number",
                table: "AppPriorities",
                column: "Number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppPriorities_Number",
                table: "AppPriorities");

            migrationBuilder.CreateIndex(
                name: "IX_AppPriorities_Number",
                table: "AppPriorities",
                column: "Number",
                unique: true);
        }
    }
}
