using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.CarModels;

public interface ICarModelAppService : IApplicationService
{
    Task<CarModelDto> UploadAsync(IFormFile file, CreateCarModelDto input);
    Task DeleteAsync(Guid id);

    Task<CarModelDto> UpdateAsync(Guid id, UpdateCarModelDto input);
    Task<CarModelDto> GetAsync(Guid id);
    Task<PagedResultDto<CarModelDto>> GetListAsync(GetCarModelListDto input);

}
