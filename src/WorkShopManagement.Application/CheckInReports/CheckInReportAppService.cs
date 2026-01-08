using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Cars;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.CheckInReports;

[RemoteService(false)]
[Authorize(WorkShopManagementPermissions.CheckInReports.Default)]
public class CheckInReportAppService : WorkShopManagementAppService, ICheckInReportAppService
{
    private readonly ICheckInReportRepository _checkInReportRepository;
    private readonly ICarRepository _carRepository;
    public CheckInReportAppService(
        ICheckInReportRepository checkInReportRepository,
        ICarRepository carRepository)
    {
            _checkInReportRepository = checkInReportRepository;
            _carRepository = carRepository;
    }

    public async Task<CheckInReportDto> CreateAsync(CreateCheckInReportDto  input)
    {
        if (input.CarId == Guid.Empty) 
        {
            throw new UserFriendlyException("CheckInReport:CarIdIsRequired");
        }

        var car = await _carRepository.GetAsync(input.CarId);
        if (car == null)
            throw new UserFriendlyException("CheckInReport:CarNotFound").WithData("CarId", input.CarId);

        var report = new CheckInReport(
            id: GuidGenerator.Create(),
            carId: input.CarId,
            buildYear: input.BuildYear,
            buildMonth: input.BuildMonth,
            avcStickerCut: input.AvcStickerCut,
            avcStickerPrinted: input.AvcStickerPrinted,
            complianceDate: input.ComplianceDate,
            compliancePlatePrinted: input.CompliancePlatePrinted,
            emission: input.Emission,
            engineNumber: input.EngineNumber,
            entryKms: input.EntryKms,
            frontGwar: input.FrontGwar,
            frontMotorNumber: input.FrontMoterNumber,
            rearGwar: input.RearGwar,
            rearMotorNumber: input.RearMotorNumber,
            maxTowingCapacity: input.MaxTowingCapacity,
            tyreLabel: input.TyreLabel,
            rsvaImportApproval: input.RsvaImportApproval,
            status: input.ReportStatus

        );

        if (input.StorageLocation.HasValue && car!.StorageLocation != input.StorageLocation)
        {
            car.SetStorageLocation(input.StorageLocation.Value);
            if (car.Stage == Stage.Incoming)
            {
                car.SetStage(Stage.ExternalWarehouse);
            }
            await _carRepository.UpdateAsync(car, autoSave: true);
        }

        report = await _checkInReportRepository.InsertAsync(report, autoSave: true);

        return ObjectMapper.Map<CheckInReport, CheckInReportDto>(report);

    }

    public async Task<CheckInReportDto?> GetByCarIdAsync(Guid carId)
    {
        var report = await _checkInReportRepository.GetByCarIdAsync(carId);
        return ObjectMapper.Map<CheckInReport, CheckInReportDto>(report);
    }

    public async Task<CheckInReportDto> GetAsync(Guid checkInReportId)
    {
        var report = await _checkInReportRepository.GetAsync(x => x.Id == checkInReportId);

        return ObjectMapper.Map<CheckInReport, CheckInReportDto>(report);
    }

    public async Task<PagedResultDto<CheckInReportDto>> GetListAsync(CheckInReportFiltersDto filter)
    {
        var filterInput = ObjectMapper.Map<CheckInReportFiltersDto, CheckInReportFiltersInput>(filter);
        var query = await _checkInReportRepository.GetListAsync(filterInput);
        var totalCount = await _checkInReportRepository.GetCountAsync(filterInput);

        return new PagedResultDto<CheckInReportDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<CheckInReport>, List<CheckInReportDto>>(query)
        };
    }

    public async Task<CheckInReportDto> UpdateAsync(Guid id, UpdateCheckInReportDto input)
    {
        var report = await _checkInReportRepository.GetAsync(id);

        var car = await _carRepository.GetAsync(report.CarId);
        if (car == null)
            throw new UserFriendlyException("CheckInReport:CarNotFound").WithData("CarId", report.CarId);

        report.AvcStickerCut = input.AvcStickerCut;
        report.AvcStickerPrinted = input.AvcStickerPrinted;

        report.ComplianceDate = input.ComplianceDate;
        report.CompliancePlatePrinted = input.CompliancePlatePrinted;

        report.Emission = input.Emission;
        report.EngineNumber = input.EngineNumber;
        report.EntryKms = input.EntryKms;

        report.FrontGwar = input.FrontGwar;
        report.FrontMoterNumber = input.FrontMoterNumber;
        report.RearGwar = input.RearGwar;
        report.RearMotorNumber = input.RearMotorNumber;

        report.MaxTowingCapacity = input.MaxTowingCapacity;
        report.TyreLabel = input.TyreLabel;

        report.RsvaImportApproval = input.RsvaImportApproval;
        report.ReportStatus = input.ReportStatus;


        if (input.StorageLocation.HasValue && car!.StorageLocation != input.StorageLocation)
        {
            car.SetStorageLocation(input.StorageLocation.Value);
            if (car.Stage == Stage.Incoming)
            {
                car.SetStage(Stage.ExternalWarehouse);
            }
            await _carRepository.UpdateAsync(car, autoSave: true);
        }

        await _checkInReportRepository.UpdateAsync(report, autoSave: true);

        return ObjectMapper.Map<CheckInReport, CheckInReportDto>(report);
    }

}
