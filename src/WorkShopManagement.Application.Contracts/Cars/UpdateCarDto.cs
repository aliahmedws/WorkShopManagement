using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.CarBays;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Cars.StorageLocations;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.Cars;

public class UpdateCarDto
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

    public StorageLocation? StorageLocation { get; set; }

    [StringLength(CarConsts.MaxBuildMaterialNumberLength)]
    public string? BuildMaterialNumber { get; set; }

    public int? AngleBailment { get; set; }
    public AvvStatus? AvvStatus { get; set; }

    [StringLength(CarConsts.MaxPdiStatusLength)]
    public string? PdiStatus { get; set; }

    [StringLength(CarConsts.ImageLinkLength)]
    public string? ImageLink { get; set; }

    // File Attachments
    public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
    public List<FileAttachmentDto> TempFiles { get; set; } = [];

}
