using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Cars;
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

    public async Task<CheckInReportDto> CreateAsync(CreateCheckInReportDto input)
    {
        if (input.CarId == Guid.Empty) 
        {
            throw new UserFriendlyException("CheckInReport:CarIdIsRequired");
        }
        if(string.IsNullOrWhiteSpace(input.VinNo))
        {
            throw new UserFriendlyException("CheckInReport:VinNoIsRequired");
        }

        var carExists = await _carRepository.AnyAsync(x => x.Id == input.CarId);
        if (!carExists)
            throw new UserFriendlyException("CheckInReport:CarNotFound").WithData("CarId", input.CarId);

        var report = new CheckInReport(
            id: GuidGenerator.Create(),
            carId: input.CarId,
            vinNo: input.VinNo.Trim(),
            buildDate: input.BuildDate,
            checkInSubmitterUser: input.CheckInSumbitterUser,
            avcStickerCut: input.AvcStickerCut,
            avcStickerPrinted: input.AvcStickerPrinted,
            complianceDate: input.ComplianceDate,
            compliancePlatePrinted: input.CompliancePlatePrinted,
            emission: input.Emission,
            engineNumber: input.EngineNumber,
            entryKms: input.EntryKms,
            frontGwar: input.FrontGwar,
            frontMotorNumber: input.FrontMoterNumbr,
            rearGwar: input.RearGwar,
            rearMotorNumber: input.RearMotorNumber,
            hsObjectId: input.HsObjectId,
            maxTowingCapacity: input.MaxTowingCapacity,
            tyreLabel: input.TyreLabel,
            rsvaImportApproval: input.RsvaImportApproval,
            status: input.Status,
            model: input.Model,
            storageLocation: input.StorageLocation
        );

         await _checkInReportRepository.InsertAsync(report, autoSave: true);

        return ObjectMapper.Map<CheckInReport, CheckInReportDto>(report);

    }

    public async Task<CheckInReportDto> GetAsync(Guid checkInReportId)
    {
        var query = await _checkInReportRepository.GetCheckInReportByIdAsync(checkInReportId);
        if (query == null) throw new EntityNotFoundException(typeof(CheckInReport), checkInReportId);

        return ObjectMapper.Map<CheckInReport, CheckInReportDto>(query);
    }

    public async Task<PagedResultDto<CheckInReportDto>> GetListAsync(CheckInReportFiltersDto filter, CancellationToken cancellationToken)
    {
        var filterInput = ObjectMapper.Map<CheckInReportFiltersDto, CheckInReportFiltersInput>(filter);
        var query = await _checkInReportRepository.GetListAsync(filterInput, cancellationToken);
        var totalCount = await _checkInReportRepository.GetCountAsync(filterInput, cancellationToken);

        return new PagedResultDto<CheckInReportDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<CheckInReport>, List<CheckInReportDto>>(query)
        };
    }

}
