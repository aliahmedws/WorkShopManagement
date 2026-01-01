using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.CarModels;
using WorkShopManagement.Cars.Exceptions;

namespace WorkShopManagement.Cars;

[Audited]
public class Car : FullAuditedAggregateRoot<Guid>
{
    public string Vin { get; private set; } = default!;
    public string Color { get; private set; } = default!;
    public Guid OwnerId { get; private set; }
    public Guid ModelId { get; private set; }
    public int ModelYear { get; private set; }

    public string? Cnc { get; private set; }
    public string? CncFirewall { get; private set; }
    public string? CncColumn { get; private set; }

    public DateTime? DueDate { get; private set; }
    public DateTime? DeliverDate { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? DueDateUpdated { get; private set; }

    public string? Notes { get; private set; }
    public string? MissingParts { get; private set; }


    public virtual CarModel? Model { get; private set; }
    public virtual CarOwner? Owner { get; private set; }

    private Car() { }

    public Car(
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
        string? missingParts = null
    ) : base(id)
    {
        SetOwner(ownerId);
        SetVin(vin);
        SetColor(color);
        SetModel(modelId);
        SetModelYear(modelYear);
        SetCnc(cnc, cncFirewall, cncColumn);
        SetSchedule(dueDate, deliverDate, startDate);
        SetNotes(notes, missingParts);
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
        ModelYear = Check.Range(modelYear, nameof(modelYear), CarConsts.MinModelYear, CarConsts.MaxModelYear);
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
}