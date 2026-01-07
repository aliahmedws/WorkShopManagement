using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Cars.Stages;

namespace WorkShopManagement.Cars;

public interface ICarRepository : IRepository<Car, Guid>
{
    Task<long> GetLongCountAsync(string? filter = null, Stage? stage = null);
    Task<List<Car>> GetListAsync(int skipCount = 0, int maxResultCount = 10, string? sorting = null, string? filter = null, Stage? stage = null);
}
