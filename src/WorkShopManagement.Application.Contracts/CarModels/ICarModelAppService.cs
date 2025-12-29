using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.CarModels;

public interface ICarModelAppService : IApplicationService
{
    Task<PagedResultDto<CarModelDto>> GetListAsync();
}
