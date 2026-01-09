using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInReportEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCheckInReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VinNo = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    AvcStickerCut = table.Column<bool>(type: "bit", nullable: true),
                    AvcStickerPrinted = table.Column<bool>(type: "bit", nullable: true),
                    BuildDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckInSumbitterUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ComplianceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompliancePlatePrinted = table.Column<bool>(type: "bit", nullable: true),
                    Emission = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EngineNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EntryKms = table.Column<int>(type: "int", nullable: true),
                    FrontGwar = table.Column<int>(type: "int", nullable: true),
                    FrontMoterNumbr = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RearGwar = table.Column<int>(type: "int", nullable: true),
                    RearMotorNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    HsObjectId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    MaxTowingCapacity = table.Column<float>(type: "real", nullable: true),
                    TyreLabel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RsvaImportApproval = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_AppCheckInReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCheckInReports_AppCars_CarId",
                        column: x => x.CarId,
                        principalTable: "AppCars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCheckInReports_CarId",
                table: "AppCheckInReports",
                column: "CarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCheckInReports");
        }
    }
}
