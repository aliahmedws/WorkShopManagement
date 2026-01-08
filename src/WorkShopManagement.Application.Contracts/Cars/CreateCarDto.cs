using System;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Cars.StorageLocations;

namespace WorkShopManagement.Cars;

public class CreateCarDto
{
    public Guid? OwnerId { get; set; }
    public CreateCarOwnerDto? Owner { get; set; }

    [Required]
    [StringLength(CarConsts.VinLength)]
    public string Vin { get; set; } = default!;

    [Required]
    [StringLength(CarConsts.MaxColorLength)]
    public string Color { get; set; } = default!;

    [Required]
    public Guid ModelId { get; set; }

    [Range(CarConsts.MinModelYear, CarConsts.MaxModelYear)]
    public int ModelYear { get; set; }
    [Required]
    public Stage Stage { get; set; } = Stage.Incoming;

    [StringLength(CarConsts.MaxCncLength)]
    public string? Cnc { get; set; }

    [StringLength(CarConsts.MaxCncFirewallLength)]
    public string? CncFirewall { get; set; }

    [StringLength(CarConsts.MaxCncColumnLength)]
    public string? CncColumn { get; set; }

    public DateTime? DueDate { get; set; }
    public DateTime? DeliverDate { get; set; }
    public DateTime? StartDate { get; set; }

    [StringLength(CarConsts.MaxNotesLength)]
    public string? Notes { get; set; }

    [StringLength(CarConsts.MaxMissingPartsLength)]
    public string? MissingParts { get; set; }
    // Transit vehicle data
    [StringLength(CarConsts.MaxLocationStatusLength)]
    public string? LocationStatus { get; set; }
    public DateTime? EtaBrisbane { get; set; }
    public DateTime? EtaScd { get; set; }
    [StringLength(CarConsts.MaxBookingNumberLength)]
    public string? BookingNumber { get; set; }
    [StringLength(CarConsts.MaxClearingAgentLength)]
    public string? ClearingAgent { get; set; }
    public StorageLocation? StorageLocation { get; set; }
}
