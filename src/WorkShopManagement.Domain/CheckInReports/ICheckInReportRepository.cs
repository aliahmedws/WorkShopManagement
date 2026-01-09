using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.CheckInReports
{
    public interface ICheckInReportRepository : IRepository<CheckInReport, Guid>
    {
        Task<CheckInReport?> GetByCarIdAsync(
        Guid carId);

        Task<List<CheckInReport>> GetListAsync(CheckInReportFiltersInput filter);

        Task<long> GetCountAsync(CheckInReportFiltersInput filter);

        Task<IQueryable<CheckInReport>> GetQueryableWithDetailsAsync(bool asNoTracking = false);
    }
}
