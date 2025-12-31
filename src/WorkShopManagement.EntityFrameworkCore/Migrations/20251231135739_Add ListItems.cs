using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddListItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppListItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CheckListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CommentPlaceholder = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    CommentType = table.Column<int>(type: "int", nullable: false),
                    IsAttachmentRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsSeparator = table.Column<bool>(type: "bit", nullable: false),
                    Attachment_Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Attachment_Path = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Attachment_FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Attachment_BlobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_AppListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppListItems_AppCheckLists_CheckListId",
                        column: x => x.CheckListId,
                        principalTable: "AppCheckLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppListItems_CheckListId",
                table: "AppListItems",
                column: "CheckListId");

            migrationBuilder.CreateIndex(
                name: "IX_AppListItems_CheckListId_Position",
                table: "AppListItems",
                columns: new[] { "CheckListId", "Position" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppListItems");
        }
    }
}
