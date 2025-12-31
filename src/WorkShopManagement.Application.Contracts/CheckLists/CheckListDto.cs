using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.CheckLists;

public class CheckListDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = default!;
    public int Position { get; set; }
    public Guid CarModelId { get; set; }
    public bool? EnableIssueItems { get; set; }
    public bool? EnableTags { get; set; } 
    public bool? EnableCheckInReport { get; set; }
}
