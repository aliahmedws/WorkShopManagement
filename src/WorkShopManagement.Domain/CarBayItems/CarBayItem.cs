using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
using WorkShopManagement.CarBays;
using WorkShopManagement.ListItems;

namespace WorkShopManagement.CarBayItems;

[Audited]
public class CarBayItem : FullAuditedAggregateRoot<Guid>
{
    public Guid CheckListItemId { get; private set; }
    public virtual ListItem? ListItem { get; private set; }

    public Guid CarBayId { get; private set; }
    public virtual CarBay? CarBay { get; private set; }

    public string? CheckRadioOption { get; private set; }
    public string? Comments { get; private set; }

    private CarBayItem() { }

    public CarBayItem(
        Guid id,
        Guid checkListItemId,
        Guid carBayId,
        string? checkRadioOption = null,
        string? comments = null
    ) : base(id)
    {
        SetCheckListItem(checkListItemId);
        SetCarBay(carBayId);
        SetCheckRadioOption(checkRadioOption);
        SetComments(comments);
    }

    public void SetCheckListItem(Guid checkListItemId)
    {
        if (checkListItemId == Guid.Empty)
            throw new UserFriendlyException("CheckListItemId cannot be empty.", nameof(checkListItemId)); 
        CheckListItemId = checkListItemId;
    }

    public void SetCarBay(Guid carBayId)
    {
        if (carBayId == Guid.Empty)
            throw new UserFriendlyException("CarBayId cannot be empty.", nameof(carBayId));

        CarBayId = carBayId;
    }

    public void SetCheckRadioOption(string? checkRadioOption)
    {
        checkRadioOption = checkRadioOption?.Trim();

        if (string.IsNullOrEmpty(checkRadioOption))
        {
            CheckRadioOption = null;
            return;
        }

        CheckRadioOption = Check.Length(
            checkRadioOption,
            nameof(checkRadioOption),
            CarBayItemConsts.MaxCheckRadioOptionLength
        );
    }

    public void SetComments(string? comments)
    {
        comments = comments?.Trim();

        if (string.IsNullOrEmpty(comments))
        {
            Comments = null;
            return;
        }

        Comments = Check.Length(
            comments,
            nameof(comments),
            CarBayItemConsts.MaxCommentsLength
        );
    }
}
