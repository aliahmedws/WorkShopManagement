using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.CarBays;

public interface ICarBayRepository : IRepository<CarBay, Guid>
{
    Task<CarBay?> FindByCarIdAsync(Guid carId, Guid bayId);
    Task<List<CarBay>> GetListAsync(
        int skipCount = 0,
        int maxResultCount = 10,
        string? sorting = null,
        string? filter = null,
        Guid? carId = null,
        Guid? bayId = null,
        bool? isActive = null);
    Task<long> GetLongCountAsync(
        string? filter = null,
        Guid? carId = null,
        Guid? bayId = null,
        bool? isActive = null);
}
