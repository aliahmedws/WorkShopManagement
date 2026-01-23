using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.CarBays;
using WorkShopManagement.EntityAttachments.FileAttachments;

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

    [Range(CarConsts.MinModelYear, int.MaxValue)]
    public int ModelYear { get; set; }

    [StringLength(CarConsts.MaxCncLength)]
    public string? Cnc { get; set; }

    [StringLength(CarConsts.MaxCncFirewallLength)]
    public string? CncFirewall { get; set; }

    [StringLength(CarConsts.MaxCncColumnLength)]
    public string? CncColumn { get; set; }

    public DateTime? DueDate { get; set; }              // ToDo remove in create / from logistics?
    public DateTime? DeliverDate { get; set; }          // ToDo remove in create / from logistics?
    public DateTime? StartDate { get; set; }             // ToDo remove in create / from logistics?

    [StringLength(CarConsts.MaxNotesLength)]
    public string? Notes { get; set; }

    [StringLength(CarConsts.MaxMissingPartsLength)]
    public string? MissingParts { get; set; }


    // Logistics vehicle data
    public Port Port { get; set; } = Port.Bne;
    public string? BookingNumber { get; set; }
    //public StorageLocation? StorageLocation { get; set; }           // Remove when creating?

    //New Added
    public string? BuildMaterialNumber { get; set; }
    public int? AngleBailment { get; set; }
    public AvvStatus? AvvStatus { get; set; }
    public string? PdiStatus { get; set; }

    [StringLength(CarConsts.ImageLinkLength)]
    public string? ImageLink { get; set; }

    public List<FileAttachmentDto> TempFiles { get; set; } = [];
}
