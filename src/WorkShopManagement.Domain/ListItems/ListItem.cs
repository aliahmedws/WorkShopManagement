using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WorkShopManagement.CheckLists;
using WorkShopManagement.FileAttachments;

namespace WorkShopManagement.ListItems;

public class ListItem : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public Guid CheckListId { get; set; }
    public virtual CheckList CheckLists { get; set; }

    public int Position { get; set; }

    public string Name { get; set; } = default!;

    public string CommentPlaceholder { get; set; } = default!;

    public CommentType CommentType { get; set; }

    public bool IsAttachmentRequired { get; set; }

    public bool IsSeparator { get; set; }

    public FileAttachment Attachment { get; set; }

    private ListItem()
    {
    }

    public ListItem(
        Guid id,
        Guid checkListId,
        int position,
        string name,
        string commentPlaceholder,
        CommentType commentType,
        bool isAttachmentRequired,
        bool isSeparator) : base(id)
    {
        CheckListId = checkListId;
        Position = position;
        Name = name;
        CommentPlaceholder = commentPlaceholder;
        CommentType = commentType;
        IsAttachmentRequired = isAttachmentRequired;
        IsSeparator = isSeparator;
    }

    public void Update(
        int position,
        string name,
        string commentPlaceholder,
        CommentType commentType,
        bool isAttachmentRequired,
        bool isSeparator)
    {
        Position = position;
        Name = name;
        CommentPlaceholder = commentPlaceholder;
        CommentType = commentType;
        IsAttachmentRequired = isAttachmentRequired;
        IsSeparator = isSeparator;
    }

    public void SetAttachment(FileAttachment attachment)
    {
        Attachment = attachment;
    }

    public void ClearAttachment()
    {
        Attachment = null;
    }
}
