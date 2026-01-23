using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.CarBays;
using WorkShopManagement.CarModels;
using WorkShopManagement.Cars.Exceptions;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Cars.StorageLocations;
using WorkShopManagement.Extensions;
using WorkShopManagement.LogisticsDetails;
using WorkShopManagement.QualityGates;
using WorkShopManagement.Recalls;
using WorkShopManagement.Utils.Helpers;

namespace WorkShopManagement.Cars;

[Audited]
public class Car : FullAuditedAggregateRoot<Guid>
{
    public string Vin { get; private set; } = default!;    
    public string Color { get; private set; } = default!;
    public Guid OwnerId { get; private set; }
    public Guid ModelId { get; private set; }
    public int ModelYear { get; set; }

    public string? Cnc { get; private set; }
    public string? CncFirewall { get; private set; }
    public string? CncColumn { get; private set; }

    public DateTime? DueDate { get; private set; }              
    public DateTime? DeliverDate { get; private set; }          
    public DateTime? StartDate { get; private set; }            // Manufacture Start Date
    public DateTime? DueDateUpdated { get; private set; }

    public string? Notes { get; private set; }
    public string? MissingParts { get; private set; }
    public Stage Stage { get; private set; } = Stage.Incoming;

    public StorageLocation? StorageLocation { get; private set; }

    // new below - move from car bay 
    public string? BuildMaterialNumber { get; private set; }

    public int? AngleBailment { get; private set; }
    public AvvStatus? AvvStatus { get; set; }
    public string? PdiStatus { get; private set; }

    //---new above from car bays

    public string? ImageLink { get; private set; }      // For Thumbnail Image with Color 


    public virtual CarModel? Model { get; private set; }
    public virtual CarOwner? Owner { get; private set; }
    public virtual ICollection<CarBay> CarBays { get; set; } = [];
    public virtual ICollection<Recall> Recalls { get; private set; } = [];
    public virtual ICollection<QualityGate> QualityGates { get; private set; } = [];
    public virtual LogisticsDetail? LogisticsDetail { get; private set; }
    private Car() { }

    internal Car(
        Guid id,
        Guid ownerId,
        string vin,
        string color,
        Guid modelId,
        int modelYear,

        string? cnc = null,
        string? cncFirewall = null,
        string? cncColumn = null,
        DateTime? dueDate = null,
        DateTime? deliverDate = null,
        DateTime? startDate = null,
        string? notes = null,
        string? missingParts = null,

        //StorageLocation? storageLocation = null,

        string? buildMaterialNumber = null,
        int? angleBailment = null,
        AvvStatus? avvStatus = null,
        string? pdiStatus = null,
        string? imageLink = null

    ) : base(id)
    {
        SetOwner(ownerId);
        SetVin(vin);
        SetColor(color);
        SetModel(modelId);
        SetModelYear(modelYear);
        Stage = Stage.Incoming;
        SetCnc(cnc, cncFirewall, cncColumn);
        SetSchedule(dueDate, deliverDate, startDate);
        SetNotes(notes, missingParts);
        //SetStorageLocation(storageLocation);
        SetBuildMaterialNumber(buildMaterialNumber);
        SetAngleBailment(angleBailment);
        SetAvvStatus(avvStatus);
        SetPdiStatus(pdiStatus);
        SetImageLink(imageLink);

    }

    public void SetOwner(Guid ownerId)
    {
        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Owner ID cannot be empty.", nameof(ownerId));
        }

        OwnerId = Check.NotNull(ownerId, nameof(ownerId));
    }

    public void SetVin(string vin)
    {
        vin = Check.NotNullOrWhiteSpace(vin, nameof(vin));
        vin = vin.Trim().ToUpperInvariant();

        if (vin.Length != CarConsts.VinLength)
        {
            throw new InvalidVinLengthException(vin);
        }

        Vin = vin;
    }

    public void SetColor(string color)
    {
        Color = Check.NotNullOrWhiteSpace(color?.Trim(), nameof(color), maxLength: CarConsts.MaxColorLength);
    }

    public void SetModel(Guid modelId)
    {
        if (modelId == Guid.Empty)
        {
            throw new ArgumentException("Model ID cannot be empty.", nameof(modelId));
        }

        ModelId = Check.NotNull(modelId, nameof(modelId));
    }

    public void SetModelYear(int modelYear)
    {
        ModelYear = Check.Range(modelYear, nameof(modelYear), CarConsts.MinModelYear);
    }

    public void SetCnc(string? cnc, string? cncFirewall, string? cncColumn)
    {
        Cnc = DomainCheck.TrimOptional(cnc, nameof(cnc), maxLength: CarConsts.MaxCncLength);
        CncFirewall = DomainCheck.TrimOptional(cncFirewall, nameof(cncFirewall), maxLength: CarConsts.MaxCncFirewallLength);
        CncColumn = DomainCheck.TrimOptional(cncColumn, nameof(cncColumn), maxLength: CarConsts.MaxCncColumnLength);
    }

    public void SetSchedule(DateTime? dueDate, DateTime? deliverDate, DateTime? startDate)
    {
        DueDate = dueDate;
        DeliverDate = deliverDate;
        StartDate = startDate;
    }

    public void SetDueDate(DateTime? dueDate, DateTime? updatedAt = null)
    {
        DueDate = dueDate;
        DueDateUpdated = updatedAt ?? DateTime.UtcNow;
    }

    public void SetNotes(string? notes, string? missingParts)
    {
        Notes = DomainCheck.TrimOptional(notes, nameof(notes), maxLength: CarConsts.MaxNotesLength);
        MissingParts = DomainCheck.TrimOptional(missingParts, nameof(missingParts), maxLength: CarConsts.MaxMissingPartsLength);
    }

    internal void SetStage(Stage stage, LogisticsDetail? logisticDetail)              
    {
        Stage = stage.EnsureDefined(nameof(Stage));
    }

    public void SetStorageLocation(StorageLocation storageLocation)        // Review this can car storage location be set to null (IF we remove from constructor)
    {
        StorageLocation = storageLocation.EnsureDefined(nameof(StorageLocation));
    }

    public void SetBuildMaterialNumber(string? buildMaterialNumber)
    {
        BuildMaterialNumber = DomainCheck.TrimOptional(
            buildMaterialNumber,
            nameof(buildMaterialNumber),
            maxLength: CarConsts.MaxBuildMaterialNumberLength
        );
    }



    public void SetAngleBailment(int? angleBailment)
    {
        AngleBailment = angleBailment; // add range check if you have a valid domain range
    }

    public void SetAvvStatus(AvvStatus? avvStatus)
    {
        AvvStatus = avvStatus;
    }

    public void SetPdiStatus(string? pdiStatus)
    {
        PdiStatus = DomainCheck.TrimOptional(
            pdiStatus,
            nameof(pdiStatus),
            maxLength: CarConsts.MaxPdiStatusLength
        );
    }

    public void SetImageLink(string? link)
    {
        if(CarHelper.TryGetValidHttpUrl(link, out var url))
        {
            ImageLink = DomainCheck.TrimOptional(url, nameof(url), maxLength: CarConsts.ImageLinkLength);
        }
    }
}