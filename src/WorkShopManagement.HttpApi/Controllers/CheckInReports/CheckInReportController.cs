using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.CheckInReports;

namespace WorkShopManagement.Controllers.CheckInReports;

[ControllerName("CheckInReport")]
[Area("app")]
[Route("api/app/check-in-reports")]
public class CheckInReportController : WorkShopManagementController, ICheckInReportAppService
{
    private readonly ICheckInReportAppService _checkInReportAppService;
    public CheckInReportController(ICheckInReportAppService checkInReportAppService)
    {
        _checkInReportAppService = checkInReportAppService;
    }

    [HttpPost]
    public async Task<CheckInReportDto> CreateAsync(CreateCheckInReportDto input)
    {
        return await _checkInReportAppService.CreateAsync(input);
    }

    [HttpGet("{checkInReportId}")]
    public async Task<CheckInReportDto> GetAsync(Guid checkInReportId)
    {
        return await _checkInReportAppService.GetAsync(checkInReportId);
    }

    [HttpGet]
    public async Task<PagedResultDto<CheckInReportDto>> GetListAsync(CheckInReportFiltersDto filter, CancellationToken cancellationToken)
    {
        return await _checkInReportAppService.GetListAsync(filter, cancellationToken);
    }
}
