using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddedLogistics_ArrivalEstimates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingNumber",
                table: "AppCars");

            migrationBuilder.DropColumn(
                name: "EtaBrisbane",
                table: "AppCars");

            migrationBuilder.DropColumn(
                name: "EtaScd",
                table: "AppCars");

            migrationBuilder.RenameColumn(
                name: "LocationStatus",
                table: "AppCars",
                newName: "PdiStatus");

            migrationBuilder.RenameColumn(
                name: "ClearingAgent",
                table: "AppCars",
                newName: "BuildMaterialNumber");

            migrationBuilder.AddColumn<Guid>(
                name: "CarId",
                table: "AppQualityGates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AngleBailment",
                table: "AppCars",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AvvStatus",
                table: "AppCars",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppLogisticsDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ClearingAgent = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ClearanceRemarks = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ClearanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreStatus = table.Column<int>(type: "int", nullable: false),
                    CreSubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RsvaNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Port = table.Column<int>(type: "int", nullable: false),
                    ActualPortArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualScdArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliverTo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ConfirmedDeliverDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliverNotes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    TransportDestination = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_AppLogisticsDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppLogisticsDetails_AppCars_CarId",
                        column: x => x.CarId,
                        principalTable: "AppCars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppArrivalEstimates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogisticsDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EtaPort = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EtaScd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
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
                    table.PrimaryKey("PK_AppArrivalEstimates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppArrivalEstimates_AppLogisticsDetails_LogisticsDetailId",
                        column: x => x.LogisticsDetailId,
                        principalTable: "AppLogisticsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppQualityGates_CarId",
                table: "AppQualityGates",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_AppArrivalEstimates_LogisticsDetailId",
                table: "AppArrivalEstimates",
                column: "LogisticsDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AppLogisticsDetails_CarId",
                table: "AppLogisticsDetails",
                column: "CarId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppQualityGates_AppCars_CarId",
                table: "AppQualityGates",
                column: "CarId",
                principalTable: "AppCars",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppQualityGates_AppCars_CarId",
                table: "AppQualityGates");

            migrationBuilder.DropTable(
                name: "AppArrivalEstimates");

            migrationBuilder.DropTable(
                name: "AppLogisticsDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppQualityGates_CarId",
                table: "AppQualityGates");

            migrationBuilder.DropColumn(
                name: "CarId",
                table: "AppQualityGates");

            migrationBuilder.DropColumn(
                name: "AngleBailment",
                table: "AppCars");

            migrationBuilder.DropColumn(
                name: "AvvStatus",
                table: "AppCars");

            migrationBuilder.RenameColumn(
                name: "PdiStatus",
                table: "AppCars",
                newName: "LocationStatus");

            migrationBuilder.RenameColumn(
                name: "BuildMaterialNumber",
                table: "AppCars",
                newName: "ClearingAgent");

            migrationBuilder.AddColumn<string>(
                name: "BookingNumber",
                table: "AppCars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EtaBrisbane",
                table: "AppCars",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EtaScd",
                table: "AppCars",
                type: "datetime2",
                nullable: true);
        }
    }
}
