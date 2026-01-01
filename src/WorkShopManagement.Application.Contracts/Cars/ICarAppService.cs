using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.Cars;

public interface ICarAppService : IApplicationService
{
    Task<CarDto> GetAsync(Guid id);
    Task<PagedResultDto<CarDto>> GetListAsync(GetCarListInput input);
    Task<CarDto> CreateAsync(CreateCarDto input);
    Task<CarDto> UpdateAsync(Guid id, UpdateCarDto input);
    Task DeleteAsync(Guid id);
}
