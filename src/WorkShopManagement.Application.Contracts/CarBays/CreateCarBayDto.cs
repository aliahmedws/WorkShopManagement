using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.CarBays;

public class CreateCarBayDto
{
    [Required]
    public Guid CarId { get; set; }

    [Required]
    public Guid BayId { get; set; }

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
}
