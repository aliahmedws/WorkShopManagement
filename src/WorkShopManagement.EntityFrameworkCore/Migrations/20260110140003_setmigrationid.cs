using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class setmigrationid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCarBayItems_AppListItems_CarBayId",
                table: "AppCarBayItems");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCarBayItems_AppListItems_CheckListItemId",
                table: "AppCarBayItems",
                column: "CheckListItemId",
                principalTable: "AppListItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCarBayItems_AppListItems_CheckListItemId",
                table: "AppCarBayItems");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCarBayItems_AppListItems_CarBayId",
                table: "AppCarBayItems",
                column: "CarBayId",
                principalTable: "AppListItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
