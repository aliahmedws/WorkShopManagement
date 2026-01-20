using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCheckInReport_FieldDataTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrontGwar",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "RearGwar",
                table: "AppCheckInReports");

            migrationBuilder.RenameColumn(
                name: "FrontMoterNumber",
                table: "AppCheckInReports",
                newName: "FrontMotorNumber");

            migrationBuilder.AlterColumn<string>(
                name: "MaxTowingCapacity",
                table: "AppCheckInReports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrontGawr",
                table: "AppCheckInReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RearGawr",
                table: "AppCheckInReports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrontGawr",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "RearGawr",
                table: "AppCheckInReports");

            migrationBuilder.RenameColumn(
                name: "FrontMotorNumber",
                table: "AppCheckInReports",
                newName: "FrontMoterNumber");

            migrationBuilder.AlterColumn<double>(
                name: "MaxTowingCapacity",
                table: "AppCheckInReports",
                type: "float",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FrontGwar",
                table: "AppCheckInReports",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RearGwar",
                table: "AppCheckInReports",
                type: "float",
                nullable: true);
        }
    }
}
