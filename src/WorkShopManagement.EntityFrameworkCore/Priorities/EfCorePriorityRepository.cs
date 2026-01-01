using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

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
            query = query.OrderBy(e => e.Number);
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

        if (!filter.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.Number.ToString().Contains(filter));
        }

        return query;
    }
}
