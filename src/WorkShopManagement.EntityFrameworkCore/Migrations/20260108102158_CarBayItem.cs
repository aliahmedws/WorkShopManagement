using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class CarBayItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCarBayItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckListItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CarBayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckRadioOption = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
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
                    table.PrimaryKey("PK_AppCarBayItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCarBayItems_AppCarBays_CarBayId",
                        column: x => x.CarBayId,
                        principalTable: "AppCarBays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppCarBayItems_AppListItems_CarBayId",
                        column: x => x.CarBayId,
                        principalTable: "AppListItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCarBayItems_CarBayId",
                table: "AppCarBayItems",
                column: "CarBayId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCarBayItems_CheckListItemId",
                table: "AppCarBayItems",
                column: "CheckListItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCarBayItems");
        }
    }
}
