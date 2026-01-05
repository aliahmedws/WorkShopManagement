using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.CheckInReports
{
    public interface ICheckInReportAppService
    {
        Task<CheckInReportDto> CreateAsync(CreateCheckInReportDto input);
        Task<CheckInReportDto> GetAsync(Guid checkInReportId);
        Task<PagedResultDto<CheckInReportDto>> GetListAsync(CheckInReportFiltersDto filter, CancellationToken cancellationToken);
    }
}
