using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
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

        if (asNoTracking)
            query = query.AsNoTracking();

        query = query
            .Include(x => x.LogisticsDetail);    // Not working

        var car = await query.FirstOrDefaultAsync(x => x.Id == id);

        return car ?? throw new EntityNotFoundException(typeof(Car), id);
    }

    public async Task<Car> GetDetailsForReportAsync(Guid id, bool asNoTracking = false)
    {
        var query = await GetQueryableAsync();

        if (asNoTracking)
            query = query.AsNoTracking();

        query = query
            .Include(x => x.Owner)
            .Include(x => x.Model).ThenInclude(x => x.ModelCategory)
            .Include(x => x.LogisticsDetail);    // Not working

        var car = await query.FirstOrDefaultAsync(x => x.Id == id);

        return car ?? throw new EntityNotFoundException(typeof(Car), id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var car = await GetAsync(id);
        if (!car.Stage.Equals(Stage.Incoming))
        {
            throw new UserFriendlyException($"Cannot Delete Car with VIN: <strong>{car.Vin}</strong>. \nThe car is not in Incoming stage.");
        }


        var dbContext = await GetDbContextAsync();
        var carBays = dbContext.CarBays.Where(cb => cb.CarId == id);

        if(carBays.Any())
        {
            throw new InvalidOperationException($"Cannot Delete Car with VIN: <strong>{car.Vin}</strong>. \nThe car was assigned to a Car Bay.");
        }

        var logisticsDetail = dbContext.LogisticsDetails.FirstOrDefault(ld => ld.CarId == id);
        var checkInReport = dbContext.CheckInReports.FirstOrDefault(cir => cir.CarId == id);
        var issues = dbContext.Issues.Where(i => i.CarId == id);
        var attachments = dbContext.EntityAttachments.Where(a => a.EntityId == id);

        if (logisticsDetail != null)
        {
            dbContext.LogisticsDetails.Remove(logisticsDetail);
        }
        if (checkInReport != null)
        {
            dbContext.CheckInReports.Remove(checkInReport);
        }
        if(issues.Any())
        {
            dbContext.Issues.RemoveRange(issues);
        }
        if (attachments.Any())
        {
            dbContext.EntityAttachments.RemoveRange(attachments);
        }
        await dbContext.SaveChangesAsync();
        await DeleteAsync(car);
    }


}
