using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.CheckLists;
using WorkShopManagement.ListItems;

namespace WorkShopManagement.FileAttachments;

[Audited]
public class EntityAttachment : FullAuditedAggregateRoot<Guid>
{
    public Guid? CheckListId { get; private set; }
    public virtual CheckList? CheckList { get; private set; }

    public Guid? ListItemId { get; private set; }
    public virtual ListItem? ListItem { get; private set; }

    public FileAttachment Attachment { get; private set; } = default!;

    private EntityAttachment() { }

    public EntityAttachment(Guid id, Guid? checkListId, Guid? listItemId, FileAttachment attachment)
        : base(id)
    {
        SetParent(checkListId, listItemId);
        Attachment = Check.NotNull(attachment, nameof(attachment));
    }

    public void SetParent(Guid? checkListId, Guid? listItemId)
    {
        var hasCheckList = checkListId.HasValue;
        var hasListItem = listItemId.HasValue;

        if (hasCheckList == hasListItem)
        {
            throw new BusinessException("Attachment must belong to exactly one parent: CheckList or ListItem.");
        }

        CheckListId = checkListId;
        ListItemId = listItemId;
    }

    public void SetAttachment(FileAttachment attachment)
    {
        Attachment = Check.NotNull(attachment, nameof(attachment));
    }
}
