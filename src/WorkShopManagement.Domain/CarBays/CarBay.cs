using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.Bays;
using WorkShopManagement.CarBayItems;
using WorkShopManagement.Cars;
using WorkShopManagement.QualityGates;

namespace WorkShopManagement.CarBays;

[Audited]
public class CarBay : FullAuditedAggregateRoot<Guid>
{
    public Guid CarId { get; private set; }
    public virtual Car? Car { get; set; }

    public Guid BayId { get; private set; }
    public virtual Bay? Bay { get; set; }

    public virtual Priority? Priority { get; set; }

    public string? BuildMaterialNumber { get; private set; }    // remove this. it is in Car

    public Guid? UserId { get; private set; }               // remove this. comes from AuditedEntity

    public DateTime? DateTimeIn { get; private set; }
    public DateTime? DateTimeOut { get; private set; }

    public bool? IsActive { get; private set; }

    public bool? IsWaiting { get; private set; }
    public bool? IsQueue { get; private set; }

    public int? AngleBailment { get; private set; }         // remove this. it is in Car

    public AvvStatus? AvvStatus { get; private set; }       // remove this. it is in Car

    public string? PdiStatus { get; private set; }         // remove this. it is in Car

    public int? DisplayBay { get; private set; }

    public float? Percentage { get; private set; }

    public DateTime? DueDate { get; private set; }                      // remove this. it is in Car
    public DateTime? DueDateUpdated { get; private set; }                   // remove this. it is in Car

    public DateTime? ConfirmedDeliverDate { get; private set; }             // remove this. it is in Car.LogisticsDetails 
    public string? ConfirmedDeliverDateNotes { get; private set; }          // remove this. it is in Car.LogisticsDetails

    public string? TransportDestination { get; private set; }               // remove this. it is in Car.LogisticsDetails

    public string? StorageLocation { get; private set; }                    // remove this. it is in Car
    public string? Row { get; private set; }
    public string? Columns { get; private set; }

    public DateTime? ReWorkDate { get; private set; }
    public DateTime? ManufactureStartDate { get; private set; }

    public string? PulseNumber { get; private set; }

    public bool? CanProgress { get; private set; }
    public bool? JobCardCompleted { get; private set; }

    public DateTime? ClockInTime { get; private set; }
    public DateTime? ClockOutTime { get; private set; }
    public ClockInStatus ClockInStatus { get; private set; } = ClockInStatus.NotClockedIn;
    public virtual ICollection<CarBayItem>? CarBayItems { get; private set; } = new List<CarBayItem>();
    public virtual ICollection<QualityGate>? QualityGates { get; set; } = new List<QualityGate>(); 

    private CarBay() { }

    public CarBay(Guid id, Guid carId, Guid bayId) : base(id)
    {
        CarId = Check.NotNull(carId, nameof(carId));
        BayId = Check.NotNull(bayId, nameof(bayId));
    }

    public void SetPriority(Priority? priority) => Priority = priority;
    public void SetBuildMaterialNumber(string? buildMaterialNumber) => BuildMaterialNumber = buildMaterialNumber?.Trim();
    public void SetUserId(Guid? userId) => UserId = userId;
    public void SetDateTimeIn(DateTime? dateTimeIn) => DateTimeIn = dateTimeIn;
    public void SetDateTimeOut(DateTime? dateTimeOut) => DateTimeOut = dateTimeOut;

    public void SetIsActive(bool? isActive) => IsActive = isActive;
    public void SetIsWaiting(bool? isWaiting) => IsWaiting = isWaiting;
    public void SetIsQueue(bool? isQueue) => IsQueue = isQueue;

    public void SetAngleBailment(int? angleBailment) => AngleBailment = angleBailment;

    public void SetAvvStatus(AvvStatus? avvStatus) => AvvStatus = avvStatus;

    public void SetPdiStatus(string? pdiStatus) => PdiStatus = pdiStatus?.Trim();

    public void SetDisplayBay(int? displayBay) => DisplayBay = displayBay;

    public void SetPercentage(float? percentage) => Percentage = percentage;

    public void SetDueDate(DateTime? dueDate) => DueDate = dueDate;
    public void SetDueDateUpdated(DateTime? dueDateUpdated) => DueDateUpdated = dueDateUpdated;

    public void SetConfirmedDeliverDate(DateTime? confirmedDeliverDate) => ConfirmedDeliverDate = confirmedDeliverDate;
    public void SetConfirmedDeliverDateNotes(string? notes) => ConfirmedDeliverDateNotes = notes?.Trim();

    public void SetTransportDestination(string? destination) => TransportDestination = destination?.Trim();

    public void SetStorageLocation(string? storageLocation) => StorageLocation = storageLocation?.Trim();
    public void SetRow(string? row) => Row = row?.Trim();
    public void SetColumns(string? columns) => Columns = columns?.Trim();

    public void SetReWorkDate(DateTime? reWorkDate) => ReWorkDate = reWorkDate;
    public void SetManufactureStartDate(DateTime? manufactureStartDate) => ManufactureStartDate = manufactureStartDate;

    public void SetPulseNumber(string? pulseNumber) => PulseNumber = pulseNumber?.Trim();

    public void SetCanProgress(bool? canProgress) => CanProgress = canProgress;
    public void SetJobCardCompleted(bool? jobCardCompleted) => JobCardCompleted = jobCardCompleted;

    public void ClockIn(DateTime clockInTime)
    {
        if (ClockInStatus == ClockInStatus.ClockedIn)
            throw new UserFriendlyException("CarBay already clocked in.");

        ClockInTime = clockInTime;
        ClockOutTime = null;
        ClockInStatus = ClockInStatus.ClockedIn;
    }

    public void ClockOut(DateTime clockOutTime)
    {
        if (ClockInStatus != ClockInStatus.ClockedIn)
            throw new UserFriendlyException("CarBay is not clocked in.");

        if (ClockInTime.HasValue && clockOutTime < ClockInTime.Value)
            throw new UserFriendlyException("Clock out time cannot be before clock in time.");

        ClockOutTime = clockOutTime;
        ClockInStatus = ClockInStatus.ClockedOut;
    }

    public void ResetClock()
    {
        ClockInTime = null;
        ClockOutTime = null;
        ClockInStatus = ClockInStatus.NotClockedIn;
    }

}
