using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.CarModels;

public interface ICarModelRepository : IRepository<CarModel, Guid>
{
    Task<CarModel?> FindByNameAsync(string name);

    Task<long> GetCountAsync(string? filter, string? name);

    Task<List<CarModel>> GetListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        string? filter,
        string? name,
        bool includeDetails = false);

    Task<CarModel?> GetByIdAsync(Guid id, bool includeDetails = false);
}
