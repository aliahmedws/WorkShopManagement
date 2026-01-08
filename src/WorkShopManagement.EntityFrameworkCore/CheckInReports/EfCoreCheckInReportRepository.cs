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

namespace WorkShopManagement.CheckInReports;

public class EfCoreCheckInReportRepository : EfCoreRepository<WorkShopManagementDbContext, CheckInReport, Guid>, ICheckInReportRepository
{
    public EfCoreCheckInReportRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider) : base(dbContextProvider)
    {

    }

    public async Task<CheckInReport?> GetByCarIdAsync(Guid carId)
    {
        return await (await GetDbSetAsync())
            .Include(x => x.Car)
            .FirstOrDefaultAsync(x => x.CarId == carId);
    }

    public async Task<List<CheckInReport>> GetListAsync(CheckInReportFiltersInput filter)
    {
        var query = await GetFilteredQueryAsync(filter, asNoTracking: true);

        return await query
            .PageBy(filter.SkipCount, filter.MaxResultCount)
            .ToListAsync();
    }

    public async Task<long> GetCountAsync(CheckInReportFiltersInput filter)
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
            (x.Car!.Vin != null && x.Car.Vin.Contains(filter.Filter!)) ||
            (x.CreatorId != null && x.CreatorId == filter.CreatorId) ||
            (x.Emission != null && x.Emission.Contains(filter.Filter!)) ||
            (x.EngineNumber != null && x.EngineNumber.Contains(filter.Filter!)) ||
            (x.FrontMoterNumber != null && x.FrontMoterNumber.Contains(filter.Filter!)))
      
       .WhereIf(!string.IsNullOrWhiteSpace(filter.ReportStatus),
           x => x.ReportStatus == filter.ReportStatus)
      
       .WhereIf(filter.BuildYear.HasValue,
           x => x.BuildYear == filter.BuildYear)
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
           x => x.CompliancePlatePrinted == filter.CompliancePlatePrinted)

        //Car Filters
        .WhereIf(!string.IsNullOrWhiteSpace(filter.Vin),
           x => x.Car!.Vin.Contains(filter.Vin!))
         .WhereIf(!string.IsNullOrWhiteSpace(filter.Model),
           x => x.Car!.Model != null && x.Car.Model.Name.Contains(filter.Model!))
        .WhereIf(filter.StorageLocation.HasValue,
           x => x.Car!.StorageLocation == filter.StorageLocation)
       ;

        return query.OrderBy(filter.Sorting ?? CheckInReportConsts.CreationTimeDesc);
    }
}
