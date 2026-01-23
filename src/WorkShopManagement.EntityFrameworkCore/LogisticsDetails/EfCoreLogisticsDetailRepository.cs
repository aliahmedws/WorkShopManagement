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

namespace WorkShopManagement.LogisticsDetails
{
    public class EfCoreLogisticsDetailRepository
    : EfCoreRepository<WorkShopManagementDbContext, LogisticsDetail, Guid>,
      ILogisticsDetailRepository
    {
        public EfCoreLogisticsDetailRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<LogisticsDetail>> GetListAsync(
            int skipCount = 0,
            int maxResultCount = 10,
            string? sorting = null,
            string? filter = null,
            Guid? carId = null
        )
        {
            var query = await GetAllAsync(filter, carId, sorting, asNoTracking: true, includeDetails: true);

            return await query
                .PageBy(skipCount, maxResultCount)
                .ToListAsync();
        }

        public async Task<long> GetLongCountAsync(string? filter = null, Guid? carId = null)
        {
            var query = await GetAllAsync(filter, carId, sorting: null, asNoTracking: true, includeDetails: false);
            return await query.LongCountAsync();
        }

        public async Task<IQueryable<LogisticsDetail>> GetAllAsync(
            string? filter = null,
            Guid? carId = null,
            string? sorting = null,
            bool asNoTracking = false,
            bool includeDetails = true
        )
        {
            var query = await GetQueryableAsync();

            if (carId.HasValue)
            {
                query = query.Where(x => x.CarId == carId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter = filter?.Trim()))
            {
                query = query.Where(x =>
                    (x.BookingNumber != null && x.BookingNumber.Contains(filter)) ||
                    (x.ClearingAgent != null && x.ClearingAgent.Contains(filter)) ||
                    (x.RvsaNumber != null && x.RvsaNumber.Contains(filter)) ||
                    x.CarId.ToString() == filter ||
                    x.Id.ToString() == filter
                );
            }

            if (includeDetails)
            {
                query = query.Include(x => x.ArrivalEstimates);
            }

            query = query.OrderBy(LogisticsDetailConsts.GetNormalizedSorting(sorting));

            return query.AsNoTrackingIf(asNoTracking);
        }

        public async Task<LogisticsDetail?> FindByCarIdAsync(Guid carId, bool includeDetails = true, bool asNoTracking = false)
        {
            var query = await GetQueryableAsync();

            if (includeDetails)
            {
                query = query.Include(x => x.ArrivalEstimates);
            }

            query = query.Where(x => x.CarId == carId);

            query = query.AsNoTrackingIf(asNoTracking);

            return await query.FirstOrDefaultAsync();
        }
    }
}
