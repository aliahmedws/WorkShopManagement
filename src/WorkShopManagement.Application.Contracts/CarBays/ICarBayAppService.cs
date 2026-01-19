using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.CarBays;

public interface ICarBayAppService : IApplicationService
{
    Task<CarBayDto> GetAsync(Guid id);
    Task<PagedResultDto<CarBayDto>> GetListAsync(GetCarBayListDto input);
    Task<CarBayDto> CreateAsync(CreateCarBayDto input);
    Task<CarBayDto> UpdateAsync(Guid id, UpdateCarBayDto input);
    Task DeleteAsync(Guid id);
   Task<CarBayDto> ToggleClockAsync(Guid id, DateTime? time = null);
    Task<List<string>> GetCarBayItemImagesAsync(Guid carBayId);
}
