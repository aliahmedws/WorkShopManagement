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

namespace WorkShopManagement.CarBays;

public class EfCoreCarBayRepository : EfCoreRepository<WorkShopManagementDbContext, CarBay, Guid>, ICarBayRepository
{
    public EfCoreCarBayRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider)
       : base(dbContextProvider)
    {
    }

    public async Task<CarBay?> FindByCarIdAsync(Guid carId, Guid bayId)
    {
        var query = await GetQueryableAsync();

        return await query
            .Include(x => x.Car)
            .Include(x => x.Bay)
            //.Include(x => x.QualityGate)
            .FirstOrDefaultAsync(x => x.CarId == carId && x.BayId == bayId);
    }


    public async Task<List<CarBay>> GetListAsync(
        int skipCount = 0,
        int maxResultCount = 10,
        string? sorting = null,
        string? filter = null,
        Guid? carId = null,
        Guid? bayId = null,
        bool? isActive = null)
    {
        var query = await GetAllAsync(filter, carId, bayId, isActive, sorting, asNoTracking: true);

        return await query
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public async Task<long> GetLongCountAsync(
        string? filter = null,
        Guid? carId = null,
        Guid? bayId = null,
        bool? isActive = null)
    {
        var query = await GetAllAsync(filter, carId, bayId, isActive, sorting: null, asNoTracking: true);
        return await query.LongCountAsync();
    }

    public async Task<IQueryable<CarBay>> GetAllAsync(
        string? filter = null,
        Guid? carId = null,
        Guid? bayId = null,
        bool? isActive = null,
        string? sorting = null,
        bool asNoTracking = false)
    {
        var query = await GetQueryableAsync();

        if (carId.HasValue)
            query = query.Where(x => x.CarId == carId.Value);

        if (bayId.HasValue)
            query = query.Where(x => x.BayId == bayId.Value);

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(filter = filter?.Trim()))
        {
            query = query.Where(x =>
                x.Id.ToString() == filter ||
                x.CarId.ToString() == filter ||
                x.BayId.ToString() == filter ||
                (x.BuildMaterialNumber != null && x.BuildMaterialNumber.Contains(filter)) ||
                (x.PdiStatus != null && x.PdiStatus.Contains(filter)) ||
                (x.PulseNumber != null && x.PulseNumber.Contains(filter)) ||
                (x.TransportDestination != null && x.TransportDestination.Contains(filter)) ||
                (x.StorageLocation != null && x.StorageLocation.Contains(filter))
            );
        }

        sorting = string.IsNullOrWhiteSpace(sorting) ? "CreationTime desc" : sorting;

        return query
            .AsNoTrackingIf(asNoTracking)
            .Include(x => x.Car)
            .Include(x => x.Bay)
            //.Include(x => x.QualityGate)
            .OrderBy(sorting);
    }
}
