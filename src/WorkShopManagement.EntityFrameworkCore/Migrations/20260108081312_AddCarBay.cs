using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCarBay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCarBays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuildMaterialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateTimeIn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateTimeOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsWaiting = table.Column<bool>(type: "bit", nullable: true),
                    IsQueue = table.Column<bool>(type: "bit", nullable: true),
                    AngleBailment = table.Column<int>(type: "int", nullable: true),
                    AvvStatus = table.Column<int>(type: "int", nullable: true),
                    PdiStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayBay = table.Column<int>(type: "int", nullable: true),
                    Percentage = table.Column<float>(type: "real", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedDeliverDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedDeliverDateNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransportDestination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Row = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Columns = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReWorkDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ManufactureStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PulseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanProgress = table.Column<bool>(type: "bit", nullable: true),
                    JobCardCompleted = table.Column<bool>(type: "bit", nullable: true),
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
                    table.PrimaryKey("PK_AppCarBays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCarBays_AppBays_BayId",
                        column: x => x.BayId,
                        principalTable: "AppBays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppCarBays_AppCars_CarId",
                        column: x => x.CarId,
                        principalTable: "AppCars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCarBays_BayId",
                table: "AppCarBays",
                column: "BayId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCarBays_CarId",
                table: "AppCarBays",
                column: "CarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCarBays");
        }
    }
}
