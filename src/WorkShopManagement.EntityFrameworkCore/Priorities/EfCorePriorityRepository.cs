using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.EntityFrameworkCore;

namespace WorkShopManagement.Priorities;

public class EfCorePriorityRepository : EfCoreRepository<WorkShopManagementDbContext, Priority, Guid>,
      IPriorityRepository
{
    public EfCorePriorityRepository(
       IDbContextProvider<WorkShopManagementDbContext> dbContextProvider)
       : base(dbContextProvider)
    {
    }
    public async Task<Priority?> FindByNumberAsync(int number)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.Number == number);
    }
    public async Task<List<Priority>> GetListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        string? filter = null
    )
    {
        var query = await GetQueryableAsync(filter);
        if (!string.IsNullOrWhiteSpace(sorting))
        {
            query = query.OrderBy(sorting);
        }
        else
        {
            query = query.OrderBy(e => e.Description);
        }
        return await query
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync();
    }
    public async Task<long> GetCountAsync(string? filter = null)
    {
        var query = await GetQueryableAsync(filter);
        return await query.LongCountAsync();
    }
    private async Task<IQueryable<Priority>> GetQueryableAsync(string? filter)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x => (x.Description != null && x.Description.Contains(filter))
            || x.Number.ToString().Contains(filter));
        }
        return query;
    }
}
