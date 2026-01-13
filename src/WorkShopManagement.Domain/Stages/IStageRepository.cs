using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Cars;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Common;

namespace WorkShopManagement.Stages;

public interface IStageRepository : IRepository<Car, Guid>
{
    Task<ListResult<StageModel>> GetStageAsync(
        Stage stage,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string? filter = null
        );
}
