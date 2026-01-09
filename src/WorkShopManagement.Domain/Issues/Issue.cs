using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.Cars;
using WorkShopManagement.Extensions;

namespace WorkShopManagement.Issues;

[Audited]
public class Issue : FullAuditedEntity<Guid>
{
    public int SrNo { get; private set; }
    public Guid CarId { get; private set; }
    public decimal XPercent { get; private set; }
    public decimal YPercent { get; private set; }
    public IssueType Type { get; private set; }
    public IssueStatus Status { get; private set; }
    public IssueOriginStage OriginStage { get; private set; }
    public IssueDeteriorationType DeteriorationType { get; private set; }
    public string Description { get; private set; } = default!;
    public string? RectificationAction { get; private set; }
    public string? RectificationNotes { get; private set; }
    public string? QualityControlAction { get; private set; }
    public string? QualityControlNotes { get; private set; }
    public string? RepairerAction { get; private set; }
    public string? RepairerNotes { get; private set; }
    public virtual Car? Car { get; private set; }

    public void SetCarId(Guid carId)
    {
        Check.NotDefaultOrNull<Guid>(carId, nameof(carId));
        CarId = carId;
    }

    public void SetSrNo(int srNo)
    {
        SrNo = Check.Range(srNo, nameof(srNo), 1, 1000);
    }

    public void SetCoordinates(decimal xPercent, decimal yPercent)
    {
        XPercent = Check.Range(xPercent, nameof(xPercent), 0, 100);
        YPercent = Check.Range(yPercent, nameof(yPercent), 0, 100);
    }

    public void SetType(IssueType type)
    {
        Type = type.EnsureDefined(nameof(Type));
    }

    public void SetStatus(IssueStatus status)
    {
        Status = status.EnsureDefined(nameof(Status));
    }

    public void SetOriginStage(IssueOriginStage originStage)
    {
        OriginStage = originStage.EnsureDefined(nameof(OriginStage));
    }

    public void SetDeteriorationType(IssueDeteriorationType deteriorationType)
    {
        DeteriorationType = deteriorationType.EnsureDefined(nameof(DeteriorationType));
    }

    public void SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(description?.Trim(), nameof(Description), IssueConsts.MaxDescriptionLength);
    }

    public void SetRectificationAction(string? rectificationAction, string? rectificationNotes)
    {
        rectificationAction = rectificationAction?.Trim();
        rectificationNotes = rectificationNotes?.Trim();
        RectificationAction = Check.Length(rectificationAction, nameof(rectificationAction), IssueConsts.MaxRectificationActionLength);
        RectificationNotes = Check.Length(rectificationNotes, nameof(rectificationNotes), IssueConsts.MaxRectificationNotesLength);
    }

    public void SetQualityControlAction(string? qualityControlAction, string? qualityControlNotes)
    {
        qualityControlAction = qualityControlAction?.Trim();
        qualityControlNotes = qualityControlNotes?.Trim();
        QualityControlAction = Check.Length(qualityControlAction, nameof(qualityControlAction), IssueConsts.MaxQualityControlActionLength);
        QualityControlNotes = Check.Length(qualityControlNotes, nameof(qualityControlNotes), IssueConsts.MaxQualityControlNotesLength);
    }

    public void SetRepairerAction(string? repairerAction, string? repairerNotes)
    {
        repairerAction = repairerAction?.Trim();
        repairerNotes = repairerNotes?.Trim();
        RepairerAction = Check.Length(repairerAction, nameof(repairerAction), IssueConsts.MaxRepairerActionLength);
        RepairerNotes = Check.Length(repairerNotes, nameof(repairerNotes), IssueConsts.MaxRepairerNotesLength);
    }

    private Issue() { }

    public Issue(
        Guid id,
        int srNo,
        Guid carId,
        decimal xPercent,
        decimal yPercent,
        IssueType type,
        IssueStatus status,
        IssueOriginStage originStage,
        IssueDeteriorationType deteriorationType,
        string description,
        string? rectificationAction = null,
        string? rectificationNotes = null,
        string? qualityControlAction = null,
        string? qualityControlNotes = null,
        string? repairerAction = null,
        string? repairerNotes = null
        ) : base(id)
    {
        SetCarId(carId);
        SetSrNo(srNo);
        SetCoordinates(xPercent, yPercent);
        SetType(type);
        SetStatus(status);
        SetOriginStage(originStage);
        SetDeteriorationType(deteriorationType);
        SetDescription(description);
        SetRectificationAction(rectificationAction, rectificationNotes);
        SetQualityControlAction(qualityControlAction, qualityControlNotes);
        SetRepairerAction(repairerAction, repairerNotes);
    }
}