using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.EntityFrameworkCore;

namespace WorkShopManagement.LogisticsDetails.ArrivalEstimates
{
    public class EfCoreArrivalEstimateRepository
    : EfCoreRepository<WorkShopManagementDbContext, ArrivalEstimate, Guid>,
      IArrivalEstimateRepository
    {
        public EfCoreArrivalEstimateRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<ArrivalEstimate>> GetListAsync(
            Guid? logisticsDetailId = null,
            int skipCount = 0,
            int maxResultCount = 10,
            string? sorting = null,
            bool asNoTracking = true
        )
        {
            var query = await GetAllAsync(logisticsDetailId, sorting, asNoTracking);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync();
        }

        public async Task<long> GetLongCountAsync(Guid? logisticsDetailId = null)
        {
            var query = await GetAllAsync(logisticsDetailId, sorting: null, asNoTracking: true);
            return await query.LongCountAsync();
        }

        public async Task<IQueryable<ArrivalEstimate>> GetAllAsync(
            Guid? logisticsDetailId = null,
            string? sorting = null,
            bool asNoTracking = false
        )
        {
            var query = await GetQueryableAsync();

            if (logisticsDetailId.HasValue)
            {
                query = query.Where(x => x.LogisticsDetailId == logisticsDetailId.Value);
            }

            // Default sorting = latest first
            sorting = (sorting?.Trim()).IsNullOrWhiteSpace()
                ? "CreationTime DESC"
                : sorting;

            query = query.OrderBy(sorting);

            return query.AsNoTrackingIf(asNoTracking);
        }

        public async Task<ArrivalEstimate?> GetLatestAsync(Guid logisticsDetailId, bool asNoTracking = true)
        {
            var query = await GetQueryableAsync();

            query = query.Where(x => x.LogisticsDetailId == logisticsDetailId);

            query = query.OrderByDescending(x => x.CreationTime);

            query = query.AsNoTrackingIf(asNoTracking);

            return await query.FirstOrDefaultAsync();
        }
    }
}
