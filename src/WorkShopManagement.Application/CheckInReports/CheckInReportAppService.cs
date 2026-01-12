using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
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
    private readonly CarManager _carManager;
    public CheckInReportAppService(
        ICheckInReportRepository checkInReportRepository,
        ICarRepository carRepository,
        CarManager carManager)
    {
        _checkInReportRepository = checkInReportRepository;
        _carRepository = carRepository;
        _carManager = carManager;
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
            compliancePlatePrinted: input.CompliancePlatePrinted,
            complianceDate: input.ComplianceDate,
            entryKms: input.EntryKms,
            engineNumber: input.EngineNumber,
            frontGwar: input.FrontGwar,
            rearGwar: input.RearGwar,
            frontMotorNumber: input.FrontMoterNumber,
            rearMotorNumber: input.RearMotorNumber,
            maxTowingCapacity: input.MaxTowingCapacity,
            emission: input.Emission,
            tyreLabel: input.TyreLabel,
            reportStatus: input.ReportStatus

        );

        if (input.StorageLocation.HasValue && car!.StorageLocation != input.StorageLocation)
        {
            if (car.Stage == Stage.Incoming)
            {
                car = await _carManager.ChangeStageAsync(car.Id, Stage.ExternalWarehouse, input.StorageLocation);
            }
            car = await _carRepository.UpdateAsync(car);
        }

        report = await _checkInReportRepository.InsertAsync(report, autoSave: true);

        return ObjectMapper.Map<CheckInReport, CheckInReportDto>(report);

    }

    public async Task<CheckInReportDto?> GetByCarIdAsync(Guid carId)
    {
        var report = await _checkInReportRepository.GetByCarIdAsync(carId);
        //await _checkInReportRepository.EnsurePropertyLoadedAsync(report, x => x.Car);

        if (report != null)
        {
            var dto = ObjectMapper.Map<CheckInReport, CheckInReportDto>(report);
            return dto;
        }
        
        return null;
    }

    public async Task<CheckInReportDto> GetAsync(Guid checkInReportId)
    {
        var report = await _checkInReportRepository.GetAsync(checkInReportId);
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

        report.SetBuildDate(input.BuildYear, input.BuildMonth);
        report.SetAvcSticker(input.AvcStickerCut, input.AvcStickerPrinted);
        report.SetCompliance(input.CompliancePlatePrinted, input.ComplianceDate);
        report.SetEntryKms(input.EntryKms);
        report.SetEngineNumber(input.EngineNumber);
        report.SetGwars(input.FrontGwar, input.RearGwar);
        report.SetMotorNumbers(input.FrontMoterNumber, input.RearMotorNumber);
        report.SetSpecs(input.MaxTowingCapacity, input.Emission, input.TyreLabel);
        report.SetStatus(input.ReportStatus);

        if (input.StorageLocation.HasValue && car!.StorageLocation != input.StorageLocation)
        {
            car.SetStorageLocation(input.StorageLocation);
            if (car.Stage == Stage.Incoming)
            {
                car = await _carManager.ChangeStageAsync(car.Id, Stage.ExternalWarehouse, input.StorageLocation);
            }
            car = await _carRepository.UpdateAsync(car);
        }

        await _checkInReportRepository.UpdateAsync(report, autoSave: true);

        return ObjectMapper.Map<CheckInReport, CheckInReportDto>(report);
    }

}
