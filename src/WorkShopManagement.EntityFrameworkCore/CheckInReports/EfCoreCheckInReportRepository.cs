using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace WorkShopManagement.CheckInReports;

public class EfCoreCheckInReportRepository : EfCoreRepository<WorkShopManagementDbContext, CheckInReport, Guid>, ICheckInReportRepository
{
    public EfCoreCheckInReportRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider) : base(dbContextProvider)
    {

    }

    public async Task<CheckInReport?> GetCheckInReportByIdAsync(Guid checkInReportId, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Include(x => x.Car)
            .FirstOrDefaultAsync(x => x.Id == checkInReportId, cancellationToken);
    }

    public async Task<List<CheckInReport>> GetListAsync(CheckInReportFiltersInput filter, CancellationToken cancellationToken = default)
    {
        var query = await GetFilteredQueryAsync(filter, asNoTracking: true);

        return await query
            .PageBy(filter.SkipCount, filter.MaxResultCount)
            .ToListAsync();
    }

    public async Task<long> GetCountAsync(CheckInReportFiltersInput filter, CancellationToken cancellationToken = default)
    {
        var query = await GetFilteredQueryAsync(filter,
            asNoTracking: true);

        return await query.LongCountAsync();
    }

    public async Task<IQueryable<CheckInReport>> GetQueryableWithDetailsAsync(bool asNoTracking = false)
    {
        return (await GetQueryableAsync())
            .AsNoTrackingIf(asNoTracking)
            .Include(x => x.Car);
    }

    private async Task<IQueryable<CheckInReport>> GetFilteredQueryAsync(CheckInReportFiltersInput filter, bool asNoTracking = false)
    {
        IQueryable<CheckInReport> query = (await GetQueryableAsync())
            .AsNoTrackingIf(asNoTracking)
            .Include(x => x.Car);

        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Filter), x =>
            (x.VinNo != null && x.VinNo.Contains(filter.Filter!)) ||
            (x.CheckInSumbitterUser != null && x.CheckInSumbitterUser.Contains(filter.Filter!)) ||
            (x.Emission != null && x.Emission.Contains(filter.Filter!)) ||
            (x.EngineNumber != null && x.EngineNumber.Contains(filter.Filter!)) ||
            (x.FrontMoterNumbr != null && x.FrontMoterNumbr.Contains(filter.Filter!)))
       .WhereIf(!string.IsNullOrWhiteSpace(filter.VinNo),
           x => x.VinNo.Contains(filter.VinNo!))
       .WhereIf(!string.IsNullOrWhiteSpace(filter.Status),
           x => x.Status == filter.Status)
       .WhereIf(!string.IsNullOrWhiteSpace(filter.Model),
           x => x.Model != null && x.Model.Contains(filter.Model!))
       .WhereIf(!string.IsNullOrWhiteSpace(filter.StorageLocation),
           x => x.StorageLocation == filter.StorageLocation)
       .WhereIf(filter.BuildDateMin.HasValue,
           x => x.BuildDate >= filter.BuildDateMin)
       .WhereIf(filter.BuildDateMax.HasValue,
           x => x.BuildDate <= filter.BuildDateMax)
       .WhereIf(filter.ComplianceDateMin.HasValue,
           x => x.ComplianceDate >= filter.ComplianceDateMin)
       .WhereIf(filter.ComplianceDateMax.HasValue,
           x => x.ComplianceDate <= filter.ComplianceDateMax)
       .WhereIf(filter.EntryKmsMin.HasValue,
           x => x.EntryKms >= filter.EntryKmsMin)
       .WhereIf(filter.EntryKmsMax.HasValue,
           x => x.EntryKms <= filter.EntryKmsMax)
       .WhereIf(filter.AvcStickerCut.HasValue,
           x => x.AvcStickerCut == filter.AvcStickerCut)
       .WhereIf(filter.CompliancePlatePrinted.HasValue,
           x => x.CompliancePlatePrinted == filter.CompliancePlatePrinted);

        return query.OrderBy(CheckInReportConsts.GetNormalizedSorting(filter.Sorting));
    }
}
