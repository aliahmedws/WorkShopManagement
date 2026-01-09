using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.EntityAttachments;

namespace WorkShopManagement.CarBayItems;

public class CarBayItemDto : FullAuditedEntityDto<Guid>
{
    public Guid CheckListItemId { get; private set; }
    public Guid CarBayId { get; private set; }
    public string? CheckRadioOption { get; private set; }
    public string? Comments { get; private set; }
    public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
}
