using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddRadioOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppEntityAttachments_AppCheckLists_CheckListId",
                table: "AppEntityAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppEntityAttachments_AppListItems_ListItemId",
                table: "AppEntityAttachments");

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "AppEntityAttachments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "EntityType",
                table: "AppEntityAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AppRadioOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
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
                    table.PrimaryKey("PK_AppRadioOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRadioOptions_AppListItems_ListItemId",
                        column: x => x.ListItemId,
                        principalTable: "AppListItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppRadioOptions_ListItemId",
                table: "AppRadioOptions",
                column: "ListItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEntityAttachments_AppCheckLists_CheckListId",
                table: "AppEntityAttachments",
                column: "CheckListId",
                principalTable: "AppCheckLists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEntityAttachments_AppListItems_ListItemId",
                table: "AppEntityAttachments",
                column: "ListItemId",
                principalTable: "AppListItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppEntityAttachments_AppCheckLists_CheckListId",
                table: "AppEntityAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppEntityAttachments_AppListItems_ListItemId",
                table: "AppEntityAttachments");

            migrationBuilder.DropTable(
                name: "AppRadioOptions");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "AppEntityAttachments");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "AppEntityAttachments");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEntityAttachments_AppCheckLists_CheckListId",
                table: "AppEntityAttachments",
                column: "CheckListId",
                principalTable: "AppCheckLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppEntityAttachments_AppListItems_ListItemId",
                table: "AppEntityAttachments",
                column: "ListItemId",
                principalTable: "AppListItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
