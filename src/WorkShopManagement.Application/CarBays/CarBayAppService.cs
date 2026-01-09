using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.CarBays;

[RemoteService(isEnabled: false)]
[Authorize(WorkShopManagementPermissions.CarBays.Default)]
public class CarBayAppService : WorkShopManagementAppService, ICarBayAppService
{
    private readonly ICarBayRepository _repository;
    private readonly CarBayManager _manager;

    public CarBayAppService(
        ICarBayRepository repository,
        CarBayManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    public async Task<CarBayDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<CarBay, CarBayDto>(entity);
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
            input.BuildMaterialNumber,
            input.UserId,
            input.QualityGateId,
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

        return ObjectMapper.Map<CarBay, CarBayDto>(entity);
    }

    [Authorize(WorkShopManagementPermissions.CarBays.Edit)]
    public async Task<CarBayDto> UpdateAsync(Guid id, UpdateCarBayDto input)
    {
        var entity = await _manager.UpdateAsync(
            id,
            input.BuildMaterialNumber,
            input.UserId,
            input.QualityGateId,
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
        await _repository.DeleteAsync(id);
    }
}