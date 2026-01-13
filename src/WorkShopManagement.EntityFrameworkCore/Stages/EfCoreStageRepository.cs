using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.Cars;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Common;
using WorkShopManagement.EntityFrameworkCore;

namespace WorkShopManagement.Stages;

public class EfCoreStageRepository : EfCoreRepository<WorkShopManagementDbContext, Car, Guid>, IStageRepository
{
    public EfCoreStageRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<ListResult<StageModel>> GetStageAsync(
        Stage stage,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string? filter = null
        )
    {
        var ctx = await GetDbContextAsync();

        var trimmedFilter = filter?.Trim();

        // Base: filter + sort + page on Cars only (fast and prevents join row explosion)
        var carsBase = ctx.Cars
            .AsNoTracking()
            .Where(c => c.Stage == stage);

        if (!string.IsNullOrWhiteSpace(trimmedFilter))
        {
            carsBase = carsBase.Where(c => c.Vin.Contains(trimmedFilter));
        }

        var pagedCars = carsBase
            .OrderBy(CarConsts.GetNormalizedSorting(sorting))
            .Skip(skipCount)
            .Take(maxResultCount);

        // Single query projection
        var query =
            from car in pagedCars

                // LEFT JOIN Owner
            join owner0 in ctx.CarOwners.AsNoTracking()
                on car.OwnerId equals owner0.Id into owners
            from owner in owners.DefaultIfEmpty()

                // LEFT JOIN Model
            join model0 in ctx.CarModels.AsNoTracking()
                on car.ModelId equals model0.Id into models
            from model in models.DefaultIfEmpty()

                // LEFT JOIN LogisticsDetail (adjust if 1:1 or 1:many)
            join log0 in ctx.LogisticsDetails.AsNoTracking()
                on car.Id equals log0.CarId into logs
            from log in logs.DefaultIfEmpty()

            select new StageModel
            {
                // -- Cars
                CarId = car.Id,
                Vin = car.Vin,
                Color = car.Color,
                StorageLocation = car.StorageLocation,
                ModelId = car.ModelId,
                LastModificationTime = car.LastModificationTime,
                AvvStatus = car.AvvStatus,
                EstimatedRelease = car.DeliverDate,
                Notes = car.Notes,
                // -- CarOwners
                OwnerName = owner != null ? owner.Name : null,

                // -- CarModels
                ModelName = model != null ? model.Name : null,

                // -- LogisticsDetail
                Port = log != null ? log.Port : null,
                BookingNumber = log != null ? log.BookingNumber : null,
                ClearingAgent = log != null ? log.ClearingAgent : null,
                CreStatus = log != null ? log.CreStatus : null,
                EtaScd = log != null
                        ? ctx.ArrivalEstimates
                        .Where(a => a.LogisticsDetailId == log.Id)
                        .OrderByDescending(a => a.CreationTime)
                        .Select(a => (DateTime?)a.EtaScd)
                        .FirstOrDefault()
                        : null,

                // -- Recalls
                RecallStatuses = ctx.Recalls.AsNoTracking().Where(r => r.CarId == car.Id).Select(r => r.Status),

                // -- CarBay (active only; adjust ordering/key as appropriate)
                CarBay = ctx.CarBays.AsNoTracking()
                    .Where(cb => cb.CarId == car.Id)
                    .OrderByDescending(cb => cb.CreationTime)  // or LastModificationTime / Id
                    .FirstOrDefault(),
            };

        return new ListResult<StageModel>
        {
            TotalCount = await carsBase.LongCountAsync(),
            Items = await query.ToListAsync()
        };
    }
}
