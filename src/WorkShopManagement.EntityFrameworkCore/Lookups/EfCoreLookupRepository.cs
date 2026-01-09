using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.CarBays;
using WorkShopManagement.EntityFrameworkCore;
using WorkShopManagement.Localization;

namespace WorkShopManagement.Lookups;

public class EfCoreLookupRepository : ILookupRepository
{
    protected readonly IDbContextProvider<WorkShopManagementDbContext> DbContextProvider;
    private readonly IStringLocalizer<WorkShopManagementResource> _L;
    public bool? IsChangeTrackingEnabled { get; set; } = false;
    public string? EntityName { get; set; } = null;
    public string ProviderName { get; set; } = default!;

    public EfCoreLookupRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider, IStringLocalizer<WorkShopManagementResource> L)
    {
        DbContextProvider = dbContextProvider;
        _L = L;
    }

    protected async Task<WorkShopManagementDbContext> GetDbContextAsync() => await DbContextProvider.GetDbContextAsync();

    public async Task<List<GuidLookup>> GetCarModelsAsync()
    {
        var ctx = await GetDbContextAsync();

        return await ctx.CarModels
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new GuidLookup(x.Id, x.Name))
            .ToListAsync();
    }

    public async Task<List<GuidLookup>> GetCarOwnersAsync()
    {
        var ctx = await GetDbContextAsync();

        return await ctx.CarOwners
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new GuidLookup(x.Id, x.Name))
            .ToListAsync();
    }

    public async Task<List<GuidLookup>> GetBaysAsync()
    {
        var ctx = await GetDbContextAsync();

        return await ctx.Bays
            .AsNoTracking()
            .OrderBy(x => x.CreationTime)
            .Select(x => new GuidLookup(x.Id, x.Name))
            .ToListAsync();
    }

    public Task<List<IntLookup>> GetPrioritiesAsync()
    {
        var items = Enum.GetValues<Priority>()
            .Select(p => new IntLookup
            {
                Value = (int)p,
                DisplayName = _L[$"Enum:Priority.{p}"]
            })
            .ToList();

        return Task.FromResult(items);
    }
}
