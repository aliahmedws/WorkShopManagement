using System;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.Stages;
using WorkShopManagement.StorageLocations;

namespace WorkShopManagement.Cars;

public class CarDto : AuditedEntityDto<Guid>
{
    public Guid ModelId { get; set; }
    public string? ModelName { get; set; }
    public Guid OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string? OwnerEmail { get; set; }
    public string? OwnerContactId { get; set; }

    public string Vin { get; set; } = default!;
    public string Color { get; set; } = default!;
    public int ModelYear { get; set; }
    public Stage Stage { get; set; } = default!;
    public string? Cnc { get; set; }
    public string? CncFirewall { get; set; }
    public string? CncColumn { get; set; }

    public DateTime? DueDate { get; set; }
    public DateTime? DeliverDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDateUpdated { get; set; }

    public string? Notes { get; set; }
    public string? MissingParts { get; set; }

    // Transit vehicle data
    public string? LocationStatus { get; set; }
    public DateTime? EtaBrisbane { get; set; }
    public DateTime? EtaScd { get; set; }
    public string? BookingNumber { get; set; }
    public string? ClearingAgent { get; set; }
    public StorageLocation? StorageLocation { get; set; }
}
