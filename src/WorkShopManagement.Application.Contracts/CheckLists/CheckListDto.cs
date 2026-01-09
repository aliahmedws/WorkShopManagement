using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.ListItems;

namespace WorkShopManagement.CheckLists;

public class CheckListDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = default!;
    public int Position { get; set; }
    public Guid CarModelId { get; set; }
    public bool? EnableIssueItems { get; set; }
    public bool? EnableTags { get; set; } 
    public bool? EnableCheckInReport { get; set; }
    public string ConcurrencyStamp { get; set; } = default!;
    public List<EntityAttachmentDto?> EntityAttachments { get; set; } = [];
    public List<ListItemDto> ListItems { get; set; } = new();
}
