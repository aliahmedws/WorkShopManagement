using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.LogisticsDetails
{
    public interface ILogisticsDetailRepository : IRepository<LogisticsDetail, Guid>
    {
        Task<List<LogisticsDetail>> GetListAsync(
            int skipCount = 0,
            int maxResultCount = 10,
            string? sorting = null,
            string? filter = null,
            Guid? carId = null
        );

        Task<long> GetLongCountAsync(string? filter = null, Guid? carId = null);

        Task<IQueryable<LogisticsDetail>> GetAllAsync(
            string? filter = null,
            Guid? carId = null,
            string? sorting = null,
            bool asNoTracking = false,
            bool includeDetails = true
        );

        Task<LogisticsDetail?> FindByCarIdAsync(Guid carId, bool includeDetails = true, bool asNoTracking = false);
    }
}
