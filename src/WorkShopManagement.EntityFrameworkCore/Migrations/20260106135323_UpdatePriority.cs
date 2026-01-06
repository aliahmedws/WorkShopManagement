using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppPriorities_Number",
                table: "AppPriorities");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppPriorities");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "AppPriorities");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AppPriorities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(360)",
                oldMaxLength: 360,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppPriorities_Number",
                table: "AppPriorities",
                column: "Number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppPriorities_Number",
                table: "AppPriorities");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AppPriorities",
                type: "nvarchar(360)",
                maxLength: 360,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppPriorities",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "AppPriorities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppPriorities_Number",
                table: "AppPriorities",
                column: "Number");
        }
    }
}
