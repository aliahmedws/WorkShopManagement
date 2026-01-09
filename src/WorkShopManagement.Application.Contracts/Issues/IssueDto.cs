using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.EntityAttachments;

namespace WorkShopManagement.Issues;

public class IssueDto : FullAuditedEntityDto<Guid>
{
    public Guid CarId { get; set; }
    public decimal XPercent { get; set; }
    public decimal YPercent { get; set; }
    public IssueStatus Status { get; set; }
    public IssueOriginStage OriginStage { get; set; }
    public IssueDeteriorationType DeteriorationType { get; set; }
    public string Description { get; set; } = default!;
    public string? RectificationAction { get; set; }
    public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
}