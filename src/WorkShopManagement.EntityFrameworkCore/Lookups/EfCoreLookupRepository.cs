using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.EntityFrameworkCore;

namespace WorkShopManagement.Lookups;

public class EfCoreLookupRepository : ILookupRepository
{
    protected readonly IDbContextProvider<WorkShopManagementDbContext> DbContextProvider;
    public bool? IsChangeTrackingEnabled { get; set; } = false;
    public string? EntityName { get; set; } = null;
    public string ProviderName { get; set; } = default!;

    public EfCoreLookupRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider)
    {
        DbContextProvider = dbContextProvider;
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
}
