using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.EntityFrameworkCore;

namespace WorkShopManagement.CarModels;

public class EfCoreCarModelRepository : EfCoreRepository<WorkShopManagementDbContext, CarModel, Guid>,
      ICarModelRepository
{
    public EfCoreCarModelRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<CarModel?> FindByNameAsync(string name)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<long> GetCountAsync(string? filter, string? name)
    {
        var query = await GetFilterAsync(filter, name);
        return await query.LongCountAsync();
    }

    public async Task<List<CarModel>> GetListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        string? filter,
        string? name,
        bool includeDetails = false)
    {
        var query = await GetFilterAsync(filter, name);

        if (includeDetails)
        {
            query = query.Include(x => x.FileAttachments);
        }

        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(CarModel.Name) + " asc" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public async Task<CarModel?> GetByIdAsync(Guid id, bool includeDetails = false)
    {
        var queryable = await GetQueryableAsync();

        if (includeDetails)
        {
            queryable = queryable.Include(x => x.FileAttachments);
        }

        return await queryable.FirstOrDefaultAsync(x => x.Id == id);
    }

    private async Task<IQueryable<CarModel>> GetFilterAsync(string? filter, string? name)
    {
        var queryable = await GetQueryableAsync();

        var query = queryable
            .WhereIf(!filter.IsNullOrWhiteSpace(),
                x => x.Name.ToLower().Contains(filter!.ToLower())
                  || (x.Description != null && x.Description.ToLower().Contains(filter.ToLower())))
            .WhereIf(!name.IsNullOrWhiteSpace(),
                x => x.Name.ToLower().Contains(name!.ToLower()));

        return query;
    }
}
