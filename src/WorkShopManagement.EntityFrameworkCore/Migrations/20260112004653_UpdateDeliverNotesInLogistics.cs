using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeliverNotesInLogistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConfirmedDeliverDateNotes",
                table: "AppLogisticsDetails",
                newName: "DeliverNotes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliverNotes",
                table: "AppLogisticsDetails",
                newName: "ConfirmedDeliverDateNotes");
        }
    }
}
