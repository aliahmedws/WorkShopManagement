using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class ModelCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ModelCategoryId",
                table: "AppCarModels",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AppModelCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileAttachments_Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FileAttachments_Path = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
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
                    table.PrimaryKey("PK_AppModelCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCarModels_ModelCategoryId",
                table: "AppCarModels",
                column: "ModelCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppModelCategories_Name",
                table: "AppModelCategories",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCarModels_AppModelCategories_ModelCategoryId",
                table: "AppCarModels",
                column: "ModelCategoryId",
                principalTable: "AppModelCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCarModels_AppModelCategories_ModelCategoryId",
                table: "AppCarModels");

            migrationBuilder.DropTable(
                name: "AppModelCategories");

            migrationBuilder.DropIndex(
                name: "IX_AppCarModels_ModelCategoryId",
                table: "AppCarModels");

            migrationBuilder.DropColumn(
                name: "ModelCategoryId",
                table: "AppCarModels");
        }
    }
}
