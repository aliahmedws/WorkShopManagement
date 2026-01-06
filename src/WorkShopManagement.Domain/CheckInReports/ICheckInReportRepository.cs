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
        Guid checkInReportId);

        Task<List<CheckInReport>> GetListAsync(CheckInReportFiltersInput filter);

        Task<long> GetCountAsync(CheckInReportFiltersInput filter);

        Task<IQueryable<CheckInReport>> GetQueryableWithDetailsAsync(bool asNoTracking = false);
    }
}
