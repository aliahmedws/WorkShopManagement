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

namespace WorkShopManagement.Cars;

public class EfCoreCarRepository : EfCoreRepository<WorkShopManagementDbContext, Car, Guid>, ICarRepository
{
    public EfCoreCarRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<Car>> GetListAsync(int skipCount = 0, int maxResultCount = 10, string? sorting = null)
    {
        var query = await GetAllAsync(asNoTracking: true);
        return await query
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public async Task<long> GetLongCountAsync()
    {
        var query = await GetAllAsync(asNoTracking: true);
        return await query.LongCountAsync();
    }

    public async Task<IQueryable<Car>> GetAllAsync(string? sorting = null, bool asNoTracking = false)
    {
        var query = await GetQueryableAsync();

        return query
            .OrderBy(CarConsts.GetDefaultSorting(sorting))
            .Include(q => q.Owner)
            .Include(q => q.Model)
            .AsNoTrackingIf(asNoTracking);
    }
}
