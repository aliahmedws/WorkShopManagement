using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

    [HttpPut("save-car-image/{carId}")]
    public Task<CarDto> SaveCarImageAsync(Guid carId, string imagelink)
    {
        return carAppService.SaveCarImageAsync(carId, imagelink);
    }

    [HttpGet("external-car-images/{carId}")]
    public Task<List<string>> GetExternalCarImagesAsync(Guid carId)
    {
        return carAppService.GetExternalCarImagesAsync(carId);
    }

    [HttpPut("change-car-stage/{id}")]
    public Task<CarDto> ChangeStageAsync(Guid id, ChangeCarStageDto input)
    {
        return carAppService.ChangeStageAsync(id, input);
    }

    [HttpPut("{id}/avvStatus")]
    public Task<CarDto> UpdateAvvStatusAsync(Guid id, UpdateCarAvvStatusDto input)
    {
        return carAppService.UpdateAvvStatusAsync(id, input);
    }

    [HttpPut("{id}/estimated-release")]
    public Task<CarDto> UpdateEstimatedReleaseAsync(Guid id, DateTime? estimatedReleaseDate)
    {
        return carAppService.UpdateEstimatedReleaseAsync(id, estimatedReleaseDate);
    }

    [HttpPut("{id}/notes")]
    public Task<CarDto> UpdateNotesAsync(Guid id, string? notes)
    {
        return carAppService.UpdateNotesAsync(id, notes);
    }
}