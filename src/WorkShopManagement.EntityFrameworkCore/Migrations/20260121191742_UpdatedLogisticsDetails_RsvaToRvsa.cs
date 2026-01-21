using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedLogisticsDetails_RsvaToRvsa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RsvaNumber",
                table: "AppLogisticsDetails",
                newName: "RvsaNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RvsaNumber",
                table: "AppLogisticsDetails",
                newName: "RsvaNumber");
        }
    }
}
