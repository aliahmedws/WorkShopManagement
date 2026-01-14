using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using WorkShopManagement.CarBays;

namespace WorkShopManagement.Controllers.CarBays;

[RemoteService(IsEnabled = true)]
[ControllerName("CarBay")]
[Area("app")]
[Route("api/app/car-bays")]
public class CarBayController : AbpController, ICarBayAppService
{
    private readonly ICarBayAppService _service;

    public CarBayController(ICarBayAppService service)
    {
        _service = service;
    }

    [HttpPost("{id}/clock-in")]
    public async Task<CarBayDto> ClockInAsync(Guid id, DateTime? clockInTime)
    {
       return await _service.ClockInAsync(id, clockInTime);
    }

    [HttpPost("{id}/clock-out")]
    public async Task<CarBayDto> ClockOutAsync(Guid id, DateTime? clockOutTime)
    {
       return await _service.ClockOutAsync(id, clockOutTime);
    }

    [HttpPost]
    public async Task<CarBayDto> CreateAsync(CreateCarBayDto input)
    {
        return await _service.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _service.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public async Task<CarBayDto> GetAsync(Guid id)
    {
        return await _service.GetAsync(id);
    }

    [HttpGet]
    public async Task<PagedResultDto<CarBayDto>> GetListAsync(GetCarBayListDto input)
    {
        return await _service.GetListAsync(input);
    }

    [HttpPost("{id}/reset-clock")]
    public Task<CarBayDto> ResetClockAsync(Guid id)
    {
        return _service.ResetClockAsync(id);
    }

    [HttpPut("{id}")]
    public async Task<CarBayDto> UpdateAsync(Guid id, UpdateCarBayDto input)
    {
        return await _service.UpdateAsync(id, input);
    }
}
