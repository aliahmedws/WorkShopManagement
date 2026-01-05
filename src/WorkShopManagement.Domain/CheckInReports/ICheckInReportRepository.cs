using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.CheckInReports
{
    public interface ICheckInReportRepository : IRepository<CheckInReport, Guid>
    {
        Task<CheckInReport?> GetCheckInReportByIdAsync(
        Guid checkInReportId,
        CancellationToken cancellationToken = default);

        Task<List<CheckInReport>> GetListAsync(CheckInReportFiltersInput filter, CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(CheckInReportFiltersInput filter, CancellationToken cancellationToken = default);

        Task<IQueryable<CheckInReport>> GetQueryableWithDetailsAsync(bool asNoTracking = false);
    }
}
