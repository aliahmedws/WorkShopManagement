using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using WorkShopManagement.Cars;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.CarBays;

[RemoteService(isEnabled: false)]
[Authorize(WorkShopManagementPermissions.CarBays.Default)]
public class CarBayAppService : WorkShopManagementAppService, ICarBayAppService
{
    private readonly ICarBayRepository _repository;
    private readonly ICarRepository _carRepository;
    private readonly CarBayManager _manager;
    private readonly CarManager _carManager;

    public CarBayAppService(
        ICarBayRepository repository,
        ICarRepository carRepository,
        CarBayManager manager,
        CarManager carManager
        )
    {
        _repository = repository;
        _carRepository = carRepository;
        _manager = manager;
        _carManager = carManager;
    }

    public async Task<CarBayDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetCarBayDetailsWithIdAsync(id);
        if (entity.CarBay == null) throw new UserFriendlyException("CarBay is empty");

        var dto = ObjectMapper.Map<CarBay, CarBayDto>(entity.CarBay);

        // TEMP Fix for car images display instead from modal ---
        var carImageLink = entity?.CarBay?.Car?.ImageLink;
        if (!string.IsNullOrWhiteSpace(carImageLink))
        {
            dto.ModelImagePath = carImageLink;
        }

        dto.CheckLists?.ForEach(cl =>
        {
            entity!.Progress.TryGetValue(cl.Id, out var progressStatus);
            cl.ProgressStatus = progressStatus;
        });

        return dto;
    }

    public async Task<PagedResultDto<CarBayDto>> GetListAsync(GetCarBayListDto input)
    {
        var totalCount = await _repository.GetLongCountAsync(
            filter: input.Filter,
            carId: input.CarId,
            bayId: input.BayId,
            isActive: input.IsActive
        );

        var items = await _repository.GetListAsync(
            skipCount: input.SkipCount,
            maxResultCount: input.MaxResultCount,
            sorting: input.Sorting,
            filter: input.Filter,
            carId: input.CarId,
            bayId: input.BayId,
            isActive: input.IsActive
        );

        var dtoItems = ObjectMapper.Map<List<CarBay>, List<CarBayDto>>(items);

        return new PagedResultDto<CarBayDto>(totalCount, dtoItems);
    }

    [Authorize(WorkShopManagementPermissions.CarBays.Create)]
    public async Task<CarBayDto> CreateAsync(CreateCarBayDto input)
    {
        var entity = await _manager.CreateAsync(
            input.CarId,
            input.BayId,
            input.Priority,
            input.BuildMaterialNumber,
            input.UserId,
            input.DateTimeIn,
            input.DateTimeOut,
            input.IsActive,
            input.IsWaiting,
            input.IsQueue,
            input.AngleBailment,
            input.AvvStatus,
            input.PdiStatus,
            input.DisplayBay,
            input.Percentage,
            input.DueDate,
            input.DueDateUpdated,
            input.ConfirmedDeliverDate,
            input.ConfirmedDeliverDateNotes,
            input.TransportDestination,
            input.StorageLocation,
            input.Row,
            input.Columns,
            input.ReWorkDate,
            input.ManufactureStartDate,
            input.PulseNumber,
            input.CanProgress,
            input.JobCardCompleted
        );

        await _carManager.ChangeStageAsync(input.CarId, Stage.Production);

        return ObjectMapper.Map<CarBay, CarBayDto>(entity);
    }

    [Authorize(WorkShopManagementPermissions.CarBays.Edit)]
    public async Task<CarBayDto> UpdateAsync(Guid id, UpdateCarBayDto input)
    {
        var entity = await _manager.UpdateAsync(
            id,
            input.Priority,
            input.BuildMaterialNumber,
            input.UserId,
            input.DateTimeIn,
            input.DateTimeOut,
            input.IsActive,
            input.IsWaiting,
            input.IsQueue,
            input.AngleBailment,
            input.AvvStatus,
            input.PdiStatus,
            input.DisplayBay,
            input.Percentage,
            input.DueDate,
            input.DueDateUpdated,
            input.ConfirmedDeliverDate,
            input.ConfirmedDeliverDateNotes,
            input.TransportDestination,
            input.StorageLocation,
            input.Row,
            input.Columns,
            input.ReWorkDate,
            input.ManufactureStartDate,
            input.PulseNumber,
            input.CanProgress,
            input.JobCardCompleted
        );

        if (!input.ConcurrencyStamp.IsNullOrWhiteSpace())
        {
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
        }

        return ObjectMapper.Map<CarBay, CarBayDto>(entity);
    }

    [Authorize(WorkShopManagementPermissions.CarBays.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var carBay = await _repository.GetAsync(id);

        carBay.SetIsActive(false);
        //carBay.SetDateTimeOut(Clock.Now); // optional: if you use this

        await _repository.UpdateAsync(carBay, autoSave: true);

        var car = await _carRepository.GetAsync(carBay.CarId);
        await _carManager.ChangeStageAsync(car, Stage.ScdWarehouse);
    }

    [Authorize(WorkShopManagementPermissions.CarBays.Edit)]
    public async Task<CarBayDto> ToggleClockAsync(Guid id, DateTime? time = null)
    {
        var entity = await _manager.ToggleClockAsync(id, time);
        return ObjectMapper.Map<CarBay, CarBayDto>(entity);
    }

    public async Task<List<string>> GetCarBayItemImagesAsync(Guid carBayId)
    {
        return await _repository.GetCarBayItemImages(carBayId);
    }
}