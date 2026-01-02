using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.EntityAttachments;

namespace WorkShopManagement.ListItems;

public class ListItemDto : FullAuditedEntityDto<Guid>
{
    public Guid CheckListId { get; set; }
    public int Position { get; set; }
    public string Name { get; set; } = default!;
    public string? CommentPlaceholder { get; set; } = default!;
    public CommentType? CommentType { get; set; }
    public bool? IsAttachmentRequired { get; set; }
    public bool? IsSeparator { get; set; }
    public string ConcurrencyStamp { get; set; } = default!;

    public List<EntityAttachmentDto?> Attachments { get; set; } = [];
}
