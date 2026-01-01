using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.Cars;

namespace WorkShopManagement.Controllers.Cars;

[ControllerName("Car")]
[Area("app")]
[Route("api/app/cars")]
public class CarController(ICarAppService carAppService) : WorkShopManagementController, ICarAppService
{
    [HttpPost]
    public Task<CarDto> CreateAsync(CreateCarDto input)
    {
        return carAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return carAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<CarDto> GetAsync(Guid id)
    {
        return carAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<CarDto>> GetListAsync(GetCarListInput input)
    {
        return carAppService.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<CarDto> UpdateAsync(Guid id, UpdateCarDto input)
    {
        return carAppService.UpdateAsync(id, input);
    }

    [HttpPost("external-car-details/{vin}")]
    public Task<ExternalCarDetailsDto> GetExternalCarDetailsAsync(string vin, string? modelYear = null)
    {
        return carAppService.GetExternalCarDetailsAsync(vin, modelYear);
    }
}