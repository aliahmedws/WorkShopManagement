using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCar_VinIsUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppCars_Vin",
                table: "AppCars");

            migrationBuilder.CreateIndex(
                name: "IX_AppCars_Vin",
                table: "AppCars",
                column: "Vin",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppCars_Vin",
                table: "AppCars");

            migrationBuilder.CreateIndex(
                name: "IX_AppCars_Vin",
                table: "AppCars",
                column: "Vin");
        }
    }
}
