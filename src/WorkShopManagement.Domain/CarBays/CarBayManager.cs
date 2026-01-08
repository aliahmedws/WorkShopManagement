using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace WorkShopManagement.CarBays;

public class CarBayManager : DomainService
{
    private readonly ICarBayRepository _carBayRepository;

    public CarBayManager(
        ICarBayRepository carBayRepository)
    {
        _carBayRepository = carBayRepository;
    }

    public virtual async Task<CarBay> CreateAsync(
        Guid carId,
        Guid bayId,
        string? buildMaterialNumber = null,
        Guid? userId = null,
        Guid? qualityGateId = null,
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
        Check.NotNull(carId, nameof(carId));
        Check.NotNull(bayId, nameof(bayId));

        var existing = await _carBayRepository.FindByCarIdAsync(carId, bayId);

        if (existing != null)
            throw new UserFriendlyException("Car bay already active for car.");

        var entity = new CarBay(GuidGenerator.Create(), carId, bayId);

        ApplyValues(
            entity,
            buildMaterialNumber,
            userId,
            qualityGateId,
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
        string? buildMaterialNumber = null,
        Guid? userId = null,
        Guid? qualityGateId = null,
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

        var existing = await _carBayRepository.FindByCarIdAsync(entity.CarId, entity.BayId);

        if (existing != null && existing.Id != entity.Id)
            throw new UserFriendlyException("Car bay already active for car.");

        ApplyValues(
            entity,
            buildMaterialNumber,
            userId,
            qualityGateId,
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
        string? buildMaterialNumber,
        Guid? userId,
        Guid? qualityGateId,
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
        entity.SetBuildMaterialNumber(buildMaterialNumber);
        entity.SetUserId(userId);
        entity.SetQualityGateId(qualityGateId);

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
}
