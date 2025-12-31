using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddFileAttachmentandaddnewcolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachment_BlobName",
                table: "AppListItems");

            migrationBuilder.DropColumn(
                name: "Attachment_FileExtension",
                table: "AppListItems");

            migrationBuilder.DropColumn(
                name: "Attachment_Name",
                table: "AppListItems");

            migrationBuilder.DropColumn(
                name: "Attachment_Path",
                table: "AppListItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppListItems");

            migrationBuilder.DropColumn(
                name: "CheckListType",
                table: "AppCheckLists");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCheckLists");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCarModels");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppBays");

            migrationBuilder.AddColumn<bool>(
                name: "EnableCheckInReport",
                table: "AppCheckLists",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnableIssueItems",
                table: "AppCheckLists",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnableTags",
                table: "AppCheckLists",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppEntityAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckListId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ListItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Attachment_Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Attachment_Path = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Attachment_FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Attachment_BlobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheckListId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ListItemId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEntityAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppEntityAttachments_AppCheckLists_CheckListId",
                        column: x => x.CheckListId,
                        principalTable: "AppCheckLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppEntityAttachments_AppCheckLists_CheckListId1",
                        column: x => x.CheckListId1,
                        principalTable: "AppCheckLists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppEntityAttachments_AppListItems_ListItemId",
                        column: x => x.ListItemId,
                        principalTable: "AppListItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppEntityAttachments_AppListItems_ListItemId1",
                        column: x => x.ListItemId1,
                        principalTable: "AppListItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEntityAttachments_CheckListId",
                table: "AppEntityAttachments",
                column: "CheckListId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEntityAttachments_CheckListId1",
                table: "AppEntityAttachments",
                column: "CheckListId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppEntityAttachments_ListItemId",
                table: "AppEntityAttachments",
                column: "ListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEntityAttachments_ListItemId1",
                table: "AppEntityAttachments",
                column: "ListItemId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEntityAttachments");

            migrationBuilder.DropColumn(
                name: "EnableCheckInReport",
                table: "AppCheckLists");

            migrationBuilder.DropColumn(
                name: "EnableIssueItems",
                table: "AppCheckLists");

            migrationBuilder.DropColumn(
                name: "EnableTags",
                table: "AppCheckLists");

            migrationBuilder.AddColumn<string>(
                name: "Attachment_BlobName",
                table: "AppListItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Attachment_FileExtension",
                table: "AppListItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Attachment_Name",
                table: "AppListItems",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Attachment_Path",
                table: "AppListItems",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppListItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CheckListType",
                table: "AppCheckLists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCheckLists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCarModels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppBays",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
