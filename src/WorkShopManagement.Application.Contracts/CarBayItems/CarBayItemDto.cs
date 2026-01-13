using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using WorkShopManagement.EntityAttachments;

namespace WorkShopManagement.CarBayItems;

public class CarBayItemDto : FullAuditedEntityDto<Guid>
{
    public Guid CheckListItemId { get; set; }
    public Guid CarBayId { get; set; }
    public string? CheckRadioOption { get; set; }
    public string? Comments { get; set; }
    public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
    public string? ConcurrencyStamp { get; set; }
    public string? ModifierName { get; set; }
    public string? CreatorName { get; set; }
}
