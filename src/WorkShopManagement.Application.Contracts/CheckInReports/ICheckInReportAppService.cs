using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.CheckInReports;

public interface ICheckInReportAppService : IApplicationService
{
    Task<CheckInReportDto> CreateAsync(CreateCheckInReportDto input);
    Task<CheckInReportDto> GetAsync(Guid checkInReportId);
    Task<PagedResultDto<CheckInReportDto>> GetListAsync(CheckInReportFiltersDto filter);
    Task<CheckInReportDto> UpdateAsync(Guid id, CreateCheckInReportDto input);
}
