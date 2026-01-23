using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.Bays;
using WorkShopManagement.Cars;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Common;
using WorkShopManagement.EntityFrameworkCore;
using WorkShopManagement.Issues;
using WorkShopManagement.Recalls;

namespace WorkShopManagement.Stages;

public class EfCoreStageRepository : EfCoreRepository<WorkShopManagementDbContext, Car, Guid>, IStageRepository
{
    public EfCoreStageRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<ListResult<StageModel>> GetStageAsync(
        Stage? stage = null,
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
            .AsNoTracking();

        if (stage.HasValue)
        {
            carsBase = carsBase.Where(c => c.Stage == stage.Value);
        }

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
                Stage = car.Stage,
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

                //-- Issues
                IssueStatuses = ctx.Issues.AsNoTracking().Where(r => r.CarId == car.Id).Select(r => r.Status),

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

    public async Task<List<StageBayModel>> GetBaysAsync()
    {
        var ctx = await GetDbContextAsync();

        // Load active bays
        var bays = await ctx.Bays
            .AsNoTracking()
            .Where(b => b.IsActive)
            .Select(b => new { b.Id, b.Name })
            .ToListAsync();

        var bayIds = bays.Select(b => b.Id).ToList();

        // Pull active car-bay rows (if multiple per bay, pick one deterministically)
        var activeRows = await (
            from carBay in ctx.CarBays.AsNoTracking()
            where bayIds.Contains(carBay.BayId) && carBay.IsActive == true
            join car in ctx.Cars.AsNoTracking() on carBay.CarId equals car.Id
            join owner in ctx.CarOwners.AsNoTracking() on car.OwnerId equals owner.Id into ownerGroup
            from owner in ownerGroup.DefaultIfEmpty()
            join model in ctx.CarModels.AsNoTracking() on car.ModelId equals model.Id into modelGroup
            from model in modelGroup.DefaultIfEmpty()
            select new
            {
                carBay,
                car,
                OwnerName = owner != null ? owner.Name : null,
                ModelName = model != null ? model.Name : null,
                ImageUrl = car.ImageLink ?? (model != null && model.FileAttachments != null ? model.FileAttachments.Path : null),       // Set image from car or from model
                ClockInTime = carBay.ClockInTime,
                ClockOutTime = carBay.ClockOutTime,
                ClockInStatus = carBay.ClockInStatus
            }
        ).ToListAsync();

        // Preload statuses in bulk (avoid correlated subqueries)
        var carIds = activeRows.Select(x => x.car.Id).Distinct().ToList();

        var recallMap = await ctx.Recalls.AsNoTracking()
            .Where(r => carIds.Contains(r.CarId))
            .GroupBy(r => r.CarId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.Status).ToList());

        var issueMap = await ctx.Issues.AsNoTracking()
            .Where(i => carIds.Contains(i.CarId))
            .GroupBy(i => i.CarId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.Status).ToList());

        // Choose one active row per bay (example: lowest Priority value wins)
        var activeByBay = activeRows
            .GroupBy(x => x.carBay.BayId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(x => x.carBay.Priority).First()
            );

        var results = new List<StageBayModel>(bays.Count);

        foreach (var bay in bays)
        {
            if (activeByBay.TryGetValue(bay.Id, out var x))
            {
                recallMap.TryGetValue(x.car.Id, out var recalls);
                issueMap.TryGetValue(x.car.Id, out var issues);

                results.Add(new StageBayModel
                {
                    CarBayId = x.carBay.Id,
                    BayId = x.carBay.BayId,
                    BayName = bay.Name,
                    Priority = x.carBay.Priority,
                    CarId = x.car.Id,
                    Vin = x.car.Vin,
                    ManufactureStartDate = x.carBay.ManufactureStartDate,
                    OwnerName = x.OwnerName,
                    ModelName = x.ModelName,
                    ImageUrl = x.ImageUrl,
                    RecallStatuses = recalls ?? new List<RecallStatus>(),
                    IssueStatuses = issues ?? new List<IssueStatus>(),
                });
            }
            else
            {
                results.Add(new StageBayModel
                {
                    BayId = bay.Id,
                    BayName = bay.Name
                });
            }
        }

        results = [.. results
                .OrderBy(x => BayHelper.GetNamePrefix(x.BayName))
                .ThenBy(x => BayHelper.GetTrailingNumberOrMax(x.BayName))
                .ThenBy(x => x.BayName)];

        return results;
    }
}
