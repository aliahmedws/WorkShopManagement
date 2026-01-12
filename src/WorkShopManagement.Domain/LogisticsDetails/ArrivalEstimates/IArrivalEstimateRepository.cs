using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.LogisticsDetails.ArrivalEstimates
{
    public interface IArrivalEstimateRepository : IRepository<ArrivalEstimate, Guid>
    {
        Task<List<ArrivalEstimate>> GetListAsync(
            Guid? logisticsDetailId = null,
            int skipCount = 0,
            int maxResultCount = 10,
            string? sorting = null,
            bool asNoTracking = true
        );

        Task<long> GetLongCountAsync(Guid? logisticsDetailId = null);

        Task<IQueryable<ArrivalEstimate>> GetAllAsync(
            Guid? logisticsDetailId = null,
            string? sorting = null,
            bool asNoTracking = false
        );

        Task<ArrivalEstimate?> GetLatestAsync(Guid logisticsDetailId, bool asNoTracking = true);
    }
}
