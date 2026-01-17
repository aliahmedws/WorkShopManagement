using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.CheckLists;
using WorkShopManagement.EntityFrameworkCore;

namespace WorkShopManagement.CarBays;

public class EfCoreCarBayRepository : EfCoreRepository<WorkShopManagementDbContext, CarBay, Guid>, ICarBayRepository
{
    public EfCoreCarBayRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider)
       : base(dbContextProvider)
    {
    }

    public async Task<CarBay?> FindActiveByCarIdAsync(Guid carId)
    {
        var query = await GetQueryableAsync();

        return await query
            .Where(x => x.CarId == carId && x.IsActive == true)
            .OrderByDescending(x => x.CreationTime)
            .FirstOrDefaultAsync();
    }

    public async Task<CarBay?> FindActiveByBayIdAsync(Guid bayId)
    {
        var query = await GetQueryableAsync();

        return await query
            .Where(x => x.BayId == bayId && x.IsActive == true)
            .OrderByDescending(x => x.CreationTime)
            .FirstOrDefaultAsync();
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

    public async Task<CarBayDetails> GetCarBayDetailsWithIdAsync(Guid CarId)
    {
        var ctx = await GetDbContextAsync();

        var query = await GetQueryableAsync();

        var entity = await query
            .Where(x => x.CarId == CarId)
            .Include(x => x.Bay)
            .Include(x => x.Car).ThenInclude(c => c!.Owner)
            .Include(x => x.Car).ThenInclude(c => c!.Model).ThenInclude(m => m!.FileAttachments)
            .Include(x => x.Car).ThenInclude(c => c!.Model).ThenInclude(m => m!.ModelCategory)
            .Include(x => x.Car).ThenInclude(c => c!.Model).ThenInclude(m => m!.CheckLists)
            .FirstOrDefaultAsync();

        var progressMap = new Dictionary<Guid, CheckListProgressStatus>();

        if (entity?.Car?.Model?.CheckLists != null)
        {
            entity.Car.Model.CheckLists = [.. entity.Car.Model.CheckLists.OrderBy(cl => cl.Position)];

            var checkListids = entity.Car.Model.CheckLists.Select(x => x.Id);

            var listItems = await ctx.ListItems.Where(li => checkListids.Contains(li.CheckListId)).Include(cli => cli.RadioOptions).ToListAsync();

            // Pull only what you need from DB
            var bayItems = await ctx.CarBayItems
                .Where(bi => bi.CarBay != null && bi.CarBay.CarId == CarId)
                .Select(bi => new
                {
                    bi.CheckListItemId,
                    bi.CheckRadioOption
                })
                .ToListAsync();

            // Fast lookup by checklist-item id
            var bayByItemId = bayItems
                .GroupBy(x => x.CheckListItemId)
                .ToDictionary(g => g.Key, g => g.First().CheckRadioOption); // assuming 0/1 row per item

            // Enumerate checklists in-memory; compute in O(total items)
            foreach (var cl in entity.Car.Model.CheckLists)
            {
                // Only actionable items (exclude separators)
                var actionableItems = listItems?.Where(li => li.CheckListId == cl.Id && li.IsSeparator != true && li.RadioOptions?.Count > 0).ToList() ?? [];

                if (actionableItems.Count == 0)
                {
                    progressMap[cl.Id] = CheckListProgressStatus.Completed;
                    continue;
                }

                var total = 0;
                var filled = 0;

                foreach (var li in actionableItems)
                {
                    total++;

                    // If no bay item exists, treat as empty
                    bayByItemId.TryGetValue(li.Id, out var option);

                    if (!option.IsNullOrWhiteSpace())
                    {
                        filled++;
                    }
                }

                var status =
                    filled == 0 ? CheckListProgressStatus.Pending :
                    filled < total ? CheckListProgressStatus.InProgress :
                    CheckListProgressStatus.Completed;

                progressMap[cl.Id] = status;
            }
        }

        return new CarBayDetails
        {
            CarBay = entity,
            Progress = progressMap
        };
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
            query = query
                .Where(x =>
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
           .Include(x => x.Bay)
           .Include(x => x.Car)
               .ThenInclude(x => x!.Owner)
           .Include(x => x.Car)
               .ThenInclude(x => x!.Model)
                   .ThenInclude(x => x!.FileAttachments)
           .Include(x => x.Car)
                .ThenInclude(x => x!.Model)
                    .ThenInclude(x => x!.ModelCategory)
           .OrderBy(sorting);

    }

    public async Task<List<string>> GetCarBayItemImages(Guid carBayId)
    {
        var ctx = await GetDbContextAsync();

        //List<Guid> itemIds = [];

        var itemIds = await ctx.CarBayItems
            .Where(x => x.CarBayId == carBayId)
            .Select(x => x.Id)
            .ToListAsync();

        return await ctx.EntityAttachments
            .Where(ea => ea.EntityType == EntityAttachments.EntityType.CarBayItem && itemIds.Contains(ea.EntityId))
            .Select(ea => ea.Attachment.Path)
            .ToListAsync();
    }
}
