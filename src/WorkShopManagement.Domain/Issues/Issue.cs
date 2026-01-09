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
    public Guid CarId { get; private set; }
    public decimal XPercent { get; private set; }
    public decimal YPercent { get; private set; }
    public IssueType Type { get; private set; }
    public IssueStatus Status { get; private set; }
    public IssueOriginStage OriginStage { get; private set; }
    public IssueDeteriorationType DeteriorationType { get; private set; }
    public string Description { get; private set; } = default!;
    public string? RectificationAction { get; private set; }
    public virtual Car? Car { get; private set; }

    public void SetCarId(Guid carId)
    {
        Check.NotDefaultOrNull<Guid>(carId, nameof(carId));
        CarId = carId;
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

    public void SetRectificationAction(string? rectificationAction)
    {
        rectificationAction = rectificationAction?.Trim();
        RectificationAction = Check.Length(rectificationAction, nameof(rectificationAction), IssueConsts.MaxRectificationActionLength);
    }

    private Issue() { }

    public Issue(
        Guid id,
        Guid carId,
        decimal xPercent,
        decimal yPercent,
        IssueType type,
        IssueStatus status,
        IssueOriginStage originStage,
        IssueDeteriorationType deteriorationType,
        string description,
        string? rectificationAction = null
        ) : base(id)
    {
        SetCarId(carId);
        SetCoordinates(xPercent, yPercent);
        SetType(type);
        SetStatus(status);
        SetOriginStage(originStage);
        SetDeteriorationType(deteriorationType);
        SetDescription(description);
        SetRectificationAction(rectificationAction);
    }
}