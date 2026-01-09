using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.CarBayItems;

public interface ICarBayItemAppService : IApplicationService
{
    Task<CarBayItemDto> GetAsync(Guid id);
    Task<PagedResultDto<CarBayItemDto>> GetListAsync(GetCarBayItemListDto input);
    Task<CarBayItemDto> CreateAsync(CreateCarBayItemDto input);
    Task<CarBayItemDto> UpdateAsync(Guid id, UpdateCarBayItemDto input);
    Task DeleteAsync(Guid id);
}
