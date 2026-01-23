using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using WorkShopManagement.Bays;
using WorkShopManagement.Cars;
using WorkShopManagement.LogisticsDetails;

namespace WorkShopManagement.CarBays;

public class CarBayManager : DomainService
{
    private readonly ICarBayRepository _carBayRepository;
    private readonly ICarRepository _carRepository;

    public CarBayManager(
        ICarBayRepository carBayRepository,
         ICarRepository carRepository)
    {
        _carBayRepository = carBayRepository;
        _carRepository = carRepository;
    }

    public virtual async Task<CarBay> CreateAsync(
        Guid carId,
        Guid bayId,
        Priority priority,
        string? buildMaterialNumber = null,
        Guid? userId = null,
        DateTime? dateTimeIn = null,
        DateTime? dateTimeOut = null,
        bool? isActive = null,
        bool? isWaiting = null,
        bool? isQueue = null,
        int? angleBailment = null,
        AvvStatus? avvStatus = null,
        string? pdiStatus = null,
        int? displayBay = null,
        float? percentage = null,
        DateTime? dueDate = null,
        DateTime? dueDateUpdated = null,
        DateTime? confirmedDeliverDate = null,
        string? confirmedDeliverDateNotes = null,
        string? transportDestination = null,
        string? storageLocation = null,
        string? row = null,
        string? columns = null,
        DateTime? reWorkDate = null,
        DateTime? manufactureStartDate = null, //assembly date
        string? pulseNumber = null,
        bool? canProgress = null,
        bool? jobCardCompleted = null)
    {
        Check.NotNull(carId, nameof(carId));
        Check.NotNull(bayId, nameof(bayId));

        //await EnsureNoOtherActiveBayForCarAsync(carId, isActive, ignoreId: null);
        await EnsureBayIsNotAlreadyActiveAsync(bayId, isActive, ignoreId: null);


        //var existing = await _carBayRepository.FindByCarIdAsync(carId, bayId);

        //if (existing != null)
        //    throw new UserFriendlyException("Car bay already active for car.");

        var entity = new CarBay(GuidGenerator.Create(), carId, bayId);

        ApplyValues(
            entity,
            priority,
            buildMaterialNumber,
            userId,
            dateTimeIn,
            dateTimeOut,
            isActive,
            isWaiting,
            isQueue,
            angleBailment,
            avvStatus,
            pdiStatus,
            displayBay,
            percentage,
            dueDate,
            dueDateUpdated,
            confirmedDeliverDate,
            confirmedDeliverDateNotes,
            transportDestination,
            storageLocation,
            row,
            columns,
            reWorkDate,
            manufactureStartDate,
            pulseNumber,
            canProgress,
            jobCardCompleted
        );

        return await _carBayRepository.InsertAsync(entity, autoSave: true);
    }

    public virtual async Task<CarBay> UpdateAsync(
        Guid id,
        Guid bayId,
        Priority priority,
        string? buildMaterialNumber = null,
        Guid? userId = null,
        DateTime? dateTimeIn = null,
        DateTime? dateTimeOut = null,
        bool? isActive = null,
        bool? isWaiting = null,
        bool? isQueue = null,
        int? angleBailment = null,
        AvvStatus? avvStatus = null,
        string? pdiStatus = null,
        int? displayBay = null,
        float? percentage = null,
        DateTime? dueDate = null,
        DateTime? dueDateUpdated = null,
        DateTime? confirmedDeliverDate = null,
        string? confirmedDeliverDateNotes = null,
        string? transportDestination = null,
        string? storageLocation = null,
        string? row = null,
        string? columns = null,
        DateTime? reWorkDate = null,
        DateTime? manufactureStartDate = null,
        string? pulseNumber = null,
        bool? canProgress = null,
        bool? jobCardCompleted = null)
    {
        var entity = await _carBayRepository.GetAsync(id);

        //await EnsureNoOtherActiveBayForCarAsync(entity.CarId, isActive, ignoreId: null);
        await EnsureBayIsNotAlreadyActiveAsync(bayId, isActive, ignoreId: null);

        entity.SetBayId(bayId);

        //var existing = await _carBayRepository.FindByCarIdAsync(entity.CarId, entity.BayId);

        //if (existing != null && existing.Id != entity.Id)
        //    throw new UserFriendlyException("Car bay already active for car.");

        ApplyValues(
            entity,
            priority,
            buildMaterialNumber,
            userId,
            dateTimeIn,
            dateTimeOut,
            isActive,
            isWaiting,
            isQueue,
            angleBailment,
            avvStatus,
            pdiStatus,
            displayBay,
            percentage,
            dueDate,
            dueDateUpdated,
            confirmedDeliverDate,
            confirmedDeliverDateNotes,
            transportDestination,
            storageLocation,
            row,
            columns,
            reWorkDate,
            manufactureStartDate,
            pulseNumber,
            canProgress,
            jobCardCompleted
        );

        return await _carBayRepository.UpdateAsync(entity, autoSave: true);
    }

    private static void ApplyValues(
    CarBay entity,
    Priority? priority,
    string? buildMaterialNumber,
    Guid? userId,
    DateTime? dateTimeIn,
    DateTime? dateTimeOut,
    bool? isActive,
    bool? isWaiting,
    bool? isQueue,
    int? angleBailment,
    AvvStatus? avvStatus,
    string? pdiStatus,
    int? displayBay,
    float? percentage,
    DateTime? dueDate,
    DateTime? dueDateUpdated,
    DateTime? confirmedDeliverDate,
    string? confirmedDeliverDateNotes,
    string? transportDestination,
    string? storageLocation,
    string? row,
    string? columns,
    DateTime? reWorkDate,
    DateTime? manufactureStartDate,
    string? pulseNumber,
    bool? canProgress,
    bool? jobCardCompleted)
    {
        entity.SetPriority(priority);

        entity.SetBuildMaterialNumber(buildMaterialNumber);
        entity.SetUserId(userId);

        entity.SetDateTimeIn(dateTimeIn);
        entity.SetDateTimeOut(dateTimeOut);

        entity.SetIsActive(isActive);
        entity.SetIsWaiting(isWaiting);
        entity.SetIsQueue(isQueue);

        entity.SetAngleBailment(angleBailment);
        entity.SetAvvStatus(avvStatus);
        entity.SetPdiStatus(pdiStatus);

        entity.SetDisplayBay(displayBay);
        entity.SetPercentage(percentage);

        entity.SetDueDate(dueDate);
        entity.SetDueDateUpdated(dueDateUpdated);

        entity.SetConfirmedDeliverDate(confirmedDeliverDate);
        entity.SetConfirmedDeliverDateNotes(confirmedDeliverDateNotes);

        entity.SetTransportDestination(transportDestination);

        entity.SetStorageLocation(storageLocation);
        entity.SetRow(row);
        entity.SetColumns(columns);

        entity.SetReWorkDate(reWorkDate);
        entity.SetManufactureStartDate(manufactureStartDate);

        entity.SetPulseNumber(pulseNumber);

        entity.SetCanProgress(canProgress);
        entity.SetJobCardCompleted(jobCardCompleted);
    }

    //remove this if we want to reassign bay.
    //private async Task EnsureNoOtherActiveBayForCarAsync(Guid carId, bool? isActive, Guid? ignoreId)
    //{
    //    if (isActive != true)
    //        return;

    //    var active = await _carBayRepository.FindActiveByCarIdAsync(carId);

    //    if (active == null)
    //        return;

    //    if (ignoreId.HasValue && active.Id == ignoreId.Value)
    //        return;

    //    throw new UserFriendlyException("Car already has an active bay.");
    //}

    private async Task EnsureBayIsNotAlreadyActiveAsync(Guid bayId, bool? isActive, Guid? ignoreId)
    {
        if (isActive != true)
            return;

        var active = await _carBayRepository.FindActiveByBayIdAsync(bayId);

        if (active == null)
            return;

        if (ignoreId.HasValue && active.Id == ignoreId.Value)
            return;

        throw new UserFriendlyException("Bay is already occupied.");
    }

    public virtual async Task<CarBay> ToggleClockAsync(Guid carBayId, DateTime? clientTime = null)
    {
        var entity = await _carBayRepository.GetAsync(carBayId);

        var time = clientTime ?? Clock.Now;

        if (entity.ClockInStatus == ClockInStatus.ClockedIn)
        {
            entity.ClockOut(time);
        }
        else
        {
            entity.ClockIn(time);
            var deliveryDate = time.AddDays(7);

            // Set delivery date on Car entity
            await SetCarDeliveryDateAsync(entity.CarId, deliveryDate, time);
        }

        return await _carBayRepository.UpdateAsync(entity, autoSave: true);
    }
    private async Task SetCarDeliveryDateAsync(Guid carId, DateTime deliveryDate, DateTime clockInTime)
    {
        var car = await _carRepository.GetAsync(carId);

        // Set the delivery date on the Car entity
        car.SetSchedule(car.DueDate, deliveryDate, clockInTime);

        // Update the car
        await _carRepository.UpdateAsync(car, autoSave: true);
    }
}
