using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddedIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppIssues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SrNo = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    XPercent = table.Column<decimal>(type: "decimal(6,3)", precision: 6, scale: 3, nullable: false),
                    YPercent = table.Column<decimal>(type: "decimal(6,3)", precision: 6, scale: 3, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OriginStage = table.Column<int>(type: "int", nullable: false),
                    DeteriorationType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    RectificationAction = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    RectificationNotes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    QualityControlAction = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    QualityControlNotes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    RepairerAction = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    RepairerNotes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
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
                    table.PrimaryKey("PK_AppIssues", x => x.Id);
                    table.CheckConstraint("CK_Issues_SrNo_Range", "[SrNo] >= 1 AND [SrNo] <= 1000");
                    table.CheckConstraint("CK_Issues_XPercent_Range", "[XPercent] >= 0 AND [XPercent] <= 100");
                    table.CheckConstraint("CK_Issues_YPercent_Range", "[YPercent] >= 0 AND [YPercent] <= 100");
                    table.ForeignKey(
                        name: "FK_AppIssues_AppCars_CarId",
                        column: x => x.CarId,
                        principalTable: "AppCars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppIssues_CarId_SrNo",
                table: "AppIssues",
                columns: new[] { "CarId", "SrNo" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppIssues");
        }
    }
}
