using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.CarModels;
using WorkShopManagement.FileAttachments;
using WorkShopManagement.ListItems;

namespace WorkShopManagement.CheckLists;

[Audited]
public class CheckList : FullAuditedAggregateRoot<Guid>
{
    public Guid CarModelId { get; set; }
    public virtual CarModel CarModels { get; set; }
    public string Name { get; set; } = default!;
    public int Position { get; set; }
    public bool? EnableIssueItems { get; set; } = false;
    public bool? EnableTags { get; set; } = false;
    public bool? EnableCheckInReport { get; set; } = false;

    public virtual ICollection<ListItem> ListItems { get; set; }
    public virtual ICollection<EntityAttachment> Attachments { get; set; } = new List<EntityAttachment>();

    private CheckList()
    {
        ListItems = new List<ListItem>();
    }

    public CheckList(Guid id, string name, int position, Guid carModelId) : base(id)
    {
        CarModelId = carModelId;
        SetName(name);
        SetPosition(position);
    }

    public CheckList ChangeName(string name)
    {
        SetName(name);
        return this;
    }

    public CheckList ChangePosition(int position)
    {
        SetPosition(position);
        return this;
    }

    private void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: CheckListConsts.NameMaxLength);
    }

    private void SetPosition(int position)
    {
        if (position < 0)
        {
            throw new UserFriendlyException("Invalid Value.");
        }

        Position = position;
    }
}
