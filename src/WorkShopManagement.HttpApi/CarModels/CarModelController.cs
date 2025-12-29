using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace WorkShopManagement.CarModels;

[RemoteService(IsEnabled = true)]
[ControllerName("CarModels")]
[Area("app")]
[Route("api/app/car-models")]
public class CarModelController : AbpController, ICarModelAppService
{
    private readonly ICarModelAppService _appService;

    public CarModelController(ICarModelAppService appService)
    {
        _appService = appService;
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _appService.DeleteAsync(id);
    }

    [HttpPut("update/{id}")]
    public async Task<CarModelDto> UpdateAsync(Guid id, UpdateCarModelDto input)
    {
        return await _appService.UpdateAsync(id, input);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("upload")]
    public async Task<CarModelDto> UploadAsync([FromForm] IFormFile file, [FromForm] CreateCarModelDto input)
    {
        return await _appService.UploadAsync(file, input);
    }

    [HttpGet("{id}")]
    public Task<CarModelDto> GetAsync(Guid id) => _appService.GetAsync(id);

    [HttpGet]
    public Task<PagedResultDto<CarModelDto>> GetListAsync(GetCarModelListDto input)
        => _appService.GetListAsync(input);

}
