using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.Cars;
using WorkShopManagement.CheckLists;

namespace WorkShopManagement.CarBays;

public class CarBayDto : EntityDto<Guid>
{
    public Guid CarId { get; set; }
    public Guid BayId { get; set; }
    public Priority Priority { get; set; }

    public string? BuildMaterialNumber { get; set; }
    public Guid? UserId { get; set; }
    public Guid? QualityGateId { get; set; }

    public DateTime? DateTimeIn { get; set; }
    public DateTime? DateTimeOut { get; set; }

    public bool? IsActive { get; set; }
    public bool? IsWaiting { get; set; }
    public bool? IsQueue { get; set; }

    public int? AngleBailment { get; set; }
    public AvvStatus? AvvStatus { get; set; }
    public string? PdiStatus { get; set; }

    public int? DisplayBay { get; set; }
    public float? Percentage { get; set; }

    public DateTime? DueDate { get; set; }
    public DateTime? DueDateUpdated { get; set; }

    public DateTime? ConfirmedDeliverDate { get; set; }
    public string? ConfirmedDeliverDateNotes { get; set; }
    public string? TransportDestination { get; set; }

    public string? StorageLocation { get; set; }
    public string? Row { get; set; }
    public string? Columns { get; set; }

    public DateTime? ReWorkDate { get; set; }
    public DateTime? ManufactureStartDate { get; set; }

    public string? PulseNumber { get; set; }

    public bool? CanProgress { get; set; }
    public bool? JobCardCompleted { get; set; }


    public string? BayName { get; set; }
    public int? BayNumber { get; set; }
    public string? CarVin { get; set; }
    public string? OwnerName { get; set; }
    public string? ModelName { get; set; }
    public string? ModelCategoryName { get; set; }

    public string? ModelImagePath { get; set; }
    public DateTime? ClockInTime { get;  set; }
    public DateTime? ClockOutTime { get;  set; }
    public ClockInStatus ClockInStatus { get;  set; }
    public Port Port { get; set; }

    public List<CheckListDto> CheckLists { get; set; } = new();

}
