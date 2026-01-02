using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.Cars;

public interface ICarRepository : IRepository<Car, Guid>
{
    Task<long> GetLongCountAsync();
    Task<List<Car>> GetListAsync(int skipCount = 0, int maxResultCount = 10, string? sorting = null);
}
