using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.Bays;
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

    //public Guid? PriorityId { get; private set; }
    //public virtual Priority? Priority { get; set; }

    public string? BuildMaterialNumber { get; private set; }

    public Guid? UserId { get; private set; }

    public Guid? QualityGateId { get; private set; }
    public virtual QualityGate? QualityGate { get; set; }

    public DateTime? DateTimeIn { get; private set; }
    public DateTime? DateTimeOut { get; private set; }

    public bool? IsActive { get; private set; }

    public bool? IsWaiting { get; private set; }
    public bool? IsQueue { get; private set; }

    public int? AngleBailment { get; private set; }

    public AvvStatus? AvvStatus { get; private set; }

    public string? PdiStatus { get; private set; }

    public int? DisplayBay { get; private set; }

    public float? Percentage { get; private set; }

    public DateTime? DueDate { get; private set; }
    public DateTime? DueDateUpdated { get; private set; }

    public DateTime? ConfirmedDeliverDate { get; private set; }
    public string? ConfirmedDeliverDateNotes { get; private set; }

    public string? TransportDestination { get; private set; }

    public string? StorageLocation { get; private set; }
    public string? Row { get; private set; }
    public string? Columns { get; private set; }

    public DateTime? ReWorkDate { get; private set; }
    public DateTime? ManufactureStartDate { get; private set; }

    public string? PulseNumber { get; private set; }

    public bool? CanProgress { get; private set; }
    public bool? JobCardCompleted { get; private set; }

    private CarBay() { }

    public CarBay(Guid id, Guid carId, Guid bayId) : base(id)
    {
        CarId = Check.NotNull(carId, nameof(carId));
        BayId = Check.NotNull(bayId, nameof(bayId));
    }

    //public void SetPriorityId(Guid? priorityId) => PriorityId = priorityId;
    public void SetBuildMaterialNumber(string? buildMaterialNumber) => BuildMaterialNumber = buildMaterialNumber?.Trim();
    public void SetUserId(Guid? userId) => UserId = userId;
    public void SetQualityGateId(Guid? qualityGateId) => QualityGateId = qualityGateId;

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
}
