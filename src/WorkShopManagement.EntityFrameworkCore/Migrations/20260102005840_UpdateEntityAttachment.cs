using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntityAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachment_BlobName",
                table: "AppEntityAttachments");

            migrationBuilder.DropColumn(
                name: "Attachment_FileExtension",
                table: "AppEntityAttachments");

            migrationBuilder.DropColumn(
                name: "FileAttachments_FileExtension",
                table: "AppCarModels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attachment_BlobName",
                table: "AppEntityAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Attachment_FileExtension",
                table: "AppEntityAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileAttachments_FileExtension",
                table: "AppCarModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
