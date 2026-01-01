using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.Cars;

[RemoteService(false)]
[Authorize(WorkShopManagementPermissions.Cars.Default)]
public class CarAppService : WorkShopManagementAppService, ICarAppService
{
    private readonly ICarRepository _carRepository;
    private readonly IRepository<CarOwner, Guid> _carOwnerRepository;

    public CarAppService(
        ICarRepository carRepository,
        IRepository<CarOwner, Guid> carOwnerRepository
        )
    {
        _carRepository = carRepository;
        _carOwnerRepository = carOwnerRepository;
    }

    public async Task<CarDto> GetAsync(Guid id)
    {
        var car = await _carRepository.GetAsync(id);
        return ObjectMapper.Map<Car, CarDto>(car);
    }

    public async Task<PagedResultDto<CarDto>> GetListAsync(GetCarListInput input)
    {
        var totalCount = await _carRepository.GetLongCountAsync();
        var items = await _carRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting);
        return new PagedResultDto<CarDto>(totalCount, ObjectMapper.Map<List<Car>, List<CarDto>>(items));
    }

    [Authorize(WorkShopManagementPermissions.Cars.Create)]
    public async Task<CarDto> CreateAsync(CreateCarDto input)
    {
        var ownerId = await ResolveOrCreateOwnerAsync(input.OwnerId, input.Owner);

        var car = new Car(
            GuidGenerator.Create(),
            ownerId,
            input.Vin,
            input.Color,
            input.ModelId,
            input.ModelYear,
            input.Cnc,
            input.CncFirewall,
            input.CncColumn,
            input.DueDate,
            input.DeliverDate,
            input.StartDate,
            input.Notes,
            input.MissingParts
        );

        await _carRepository.InsertAsync(car, autoSave: true);

        return ObjectMapper.Map<Car, CarDto>(car);
    }

    [Authorize(WorkShopManagementPermissions.Cars.Edit)]
    public async Task<CarDto> UpdateAsync(Guid id, UpdateCarDto input)
    {
        var car = await _carRepository.GetAsync(id);
        var ownerId = await ResolveOrCreateOwnerAsync(input.OwnerId, input.Owner);

        // Keep invariants inside aggregate
        car.SetVin(input.Vin);
        car.SetOwner(ownerId);
        car.SetColor(input.Color);
        car.SetModel(input.ModelId);
        car.SetModelYear(input.ModelYear);
        car.SetCnc(input.Cnc, input.CncFirewall, input.CncColumn);
        car.SetSchedule(input.DueDate, input.DeliverDate, input.StartDate);
        car.SetNotes(input.Notes, input.MissingParts);

        await _carRepository.UpdateAsync(car, autoSave: true);

        return ObjectMapper.Map<Car, CarDto>(car);
    }

    [Authorize(WorkShopManagementPermissions.Cars.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _carRepository.DeleteAsync(id);
    }

    private async Task<Guid> ResolveOrCreateOwnerAsync(Guid? carOwnerId, CreateCarOwnerDto? ownerDto)
    {
        // Case A: Front-end provides CarOwnerId
        if (carOwnerId.HasValue && carOwnerId.Value != Guid.Empty)
        {
            var exists = await _carOwnerRepository.AnyAsync(x => x.Id == carOwnerId.Value);
            if (!exists)
            {
                // Owner id provided, but not found; require caller to send owner details.
                throw new UserFriendlyException("Car owner not found. Provide valid CarOwnerId or provide CarOwner details to create a new owner.");
            }

            return carOwnerId.Value;
        }

        // Case B: No id provided => must provide owner details
        if (ownerDto is null)
        {
            throw new UserFriendlyException("CarOwnerId or CarOwner details are required.");
        }

        var newOwner = new CarOwner(GuidGenerator.Create(), ownerDto.Name, ownerDto.Email, ownerDto.ContactId);

        await _carOwnerRepository.InsertAsync(newOwner, autoSave: true);

        return newOwner.Id;
    }
}
