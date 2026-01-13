using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.EntityFrameworkCore;

namespace WorkShopManagement.Cars;

public class EfCoreCarRepository : EfCoreRepository<WorkShopManagementDbContext, Car, Guid>, ICarRepository
{
    public EfCoreCarRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<Car>> GetListAsync(int skipCount = 0, int maxResultCount = 10, string? sorting = null, string? filter = null, Stage? stage = null)
    {
        var query = await GetAllAsync(filter, stage, sorting, asNoTracking: true);
        return await query
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public async Task<long> GetLongCountAsync(string? filter = null, Stage? stage = null)
    {
        var query = await GetAllAsync(filter, stage, sorting: null, asNoTracking: true);
        return await query.LongCountAsync();
    }

    public async Task<IQueryable<Car>> GetAllAsync(string? filter = null, Stage? stage = null, string? sorting = null, bool asNoTracking = false)
    {
        var query = await GetQueryableAsync();

        if (stage.HasValue)
        {
            query = query.Where(q => q.Stage == stage.Value);

            if (stage.Value == Stage.PostProduction)
            {
                var dbContext = await GetDbContextAsync();

                query = query.Where(c =>
                    dbContext.CarBays.Any(cb =>
                        cb.CarId == c.Id));
            }
        }

        if (!string.IsNullOrWhiteSpace(filter = filter?.Trim()))
        {
            query = query.Where(q =>
            q.Vin.Contains(filter) ||
            q.Color.Contains(filter) ||
            q.ModelYear.ToString().Contains(filter) ||
            q.Id.ToString() == filter
            );
        }

        return query
            .AsNoTrackingIf(asNoTracking)
            .Include(q => q.Owner)
            .Include(q => q.Model)
            .OrderBy(CarConsts.GetNormalizedSorting(sorting));
    }

    public async Task<Car> GetWithDetailsAsync(Guid id, bool asNoTracking = false)
    {
        var query = await GetQueryableAsync();

        query = query
            .Include(x => x.LogisticsDetail);    // Not working

        if (asNoTracking)
            query = query.AsNoTracking();

        var car = await query.FirstOrDefaultAsync(x => x.Id == id);

        return car ?? throw new EntityNotFoundException(typeof(Car), id);
    }


}
