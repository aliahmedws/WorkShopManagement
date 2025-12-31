using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class updateentityattachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppEntityAttachments_AppCheckLists_CheckListId1",
                table: "AppEntityAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppEntityAttachments_AppListItems_ListItemId1",
                table: "AppEntityAttachments");

            migrationBuilder.DropIndex(
                name: "IX_AppEntityAttachments_CheckListId1",
                table: "AppEntityAttachments");

            migrationBuilder.DropIndex(
                name: "IX_AppEntityAttachments_ListItemId1",
                table: "AppEntityAttachments");

            migrationBuilder.DropColumn(
                name: "CheckListId1",
                table: "AppEntityAttachments");

            migrationBuilder.DropColumn(
                name: "ListItemId1",
                table: "AppEntityAttachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CheckListId1",
                table: "AppEntityAttachments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ListItemId1",
                table: "AppEntityAttachments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppEntityAttachments_CheckListId1",
                table: "AppEntityAttachments",
                column: "CheckListId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppEntityAttachments_ListItemId1",
                table: "AppEntityAttachments",
                column: "ListItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEntityAttachments_AppCheckLists_CheckListId1",
                table: "AppEntityAttachments",
                column: "CheckListId1",
                principalTable: "AppCheckLists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEntityAttachments_AppListItems_ListItemId1",
                table: "AppEntityAttachments",
                column: "ListItemId1",
                principalTable: "AppListItems",
                principalColumn: "Id");
        }
    }
}
