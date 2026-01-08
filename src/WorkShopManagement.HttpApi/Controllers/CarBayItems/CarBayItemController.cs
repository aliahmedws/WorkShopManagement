using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using WorkShopManagement.CarBayItems;

namespace WorkShopManagement.Controllers.CarBayItems;


[RemoteService(IsEnabled = true)]
[ControllerName("CarBayItem")]
[Area("app")]
[Route("api/app/car-bay-items")]
public class CarBayItemController : AbpController, ICarBayItemAppService
{
    private readonly ICarBayItemAppService _service;

    public CarBayItemController(ICarBayItemAppService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<CarBayItemDto> CreateAsync(CreateCarBayItemDto input)
    {
        return await _service.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _service.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public async Task<CarBayItemDto> GetAsync(Guid id)
    {
        return await _service.GetAsync(id);
    }

    [HttpGet]
    public async Task<PagedResultDto<CarBayItemDto>> GetListAsync(GetCarBayItemListDto input)
    {
        return await _service.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<CarBayItemDto> UpdateAsync(Guid id, UpdateCarBayItemDto input)
    {
        return await _service.UpdateAsync(id, input);
    }
}
