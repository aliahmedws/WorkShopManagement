using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCheckInReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildDate",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "CheckInSumbitterUser",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "FrontMoterNumbr",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "HsObjectId",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "StorageLocation",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "VinNo",
                table: "AppCheckInReports");

            migrationBuilder.AlterColumn<string>(
                name: "TyreLabel",
                table: "AppCheckInReports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RsvaImportApproval",
                table: "AppCheckInReports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RearMotorNumber",
                table: "AppCheckInReports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "RearGwar",
                table: "AppCheckInReports",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "MaxTowingCapacity",
                table: "AppCheckInReports",
                type: "float",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "FrontGwar",
                table: "AppCheckInReports",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EngineNumber",
                table: "AppCheckInReports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Emission",
                table: "AppCheckInReports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompliancePlatePrinted",
                table: "AppCheckInReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AvcStickerPrinted",
                table: "AppCheckInReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AvcStickerCut",
                table: "AppCheckInReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BuildMonth",
                table: "AppCheckInReports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BuildYear",
                table: "AppCheckInReports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrontMoterNumber",
                table: "AppCheckInReports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportStatus",
                table: "AppCheckInReports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildMonth",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "BuildYear",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "FrontMoterNumber",
                table: "AppCheckInReports");

            migrationBuilder.DropColumn(
                name: "ReportStatus",
                table: "AppCheckInReports");

            migrationBuilder.AlterColumn<string>(
                name: "TyreLabel",
                table: "AppCheckInReports",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RsvaImportApproval",
                table: "AppCheckInReports",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RearMotorNumber",
                table: "AppCheckInReports",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RearGwar",
                table: "AppCheckInReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "MaxTowingCapacity",
                table: "AppCheckInReports",
                type: "real",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FrontGwar",
                table: "AppCheckInReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EngineNumber",
                table: "AppCheckInReports",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Emission",
                table: "AppCheckInReports",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "CompliancePlatePrinted",
                table: "AppCheckInReports",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AvcStickerPrinted",
                table: "AppCheckInReports",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AvcStickerCut",
                table: "AppCheckInReports",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BuildDate",
                table: "AppCheckInReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckInSumbitterUser",
                table: "AppCheckInReports",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrontMoterNumbr",
                table: "AppCheckInReports",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HsObjectId",
                table: "AppCheckInReports",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "AppCheckInReports",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AppCheckInReports",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageLocation",
                table: "AppCheckInReports",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VinNo",
                table: "AppCheckInReports",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}
