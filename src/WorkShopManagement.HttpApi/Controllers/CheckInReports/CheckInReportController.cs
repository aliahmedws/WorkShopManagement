using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using WorkShopManagement.CheckInReports;

namespace WorkShopManagement.Controllers.CheckInReports;

[ControllerName("CheckInReport")]
[Area("app")]
[Route("api/app/check-in-reports")]
public class CheckInReportController : AbpController, ICheckInReportAppService
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

    [HttpGet("{id}")]
    public async Task<CheckInReportDto> GetAsync(Guid checkInReportId)
    {
        return await _checkInReportAppService.GetAsync(checkInReportId);
    }

    [HttpGet("Get-List")]
    public async Task<PagedResultDto<CheckInReportDto>> GetListAsync(CheckInReportFiltersDto filter, CancellationToken cancellationToken)
    {
        return await _checkInReportAppService.GetListAsync(filter, cancellationToken);
    }
}
