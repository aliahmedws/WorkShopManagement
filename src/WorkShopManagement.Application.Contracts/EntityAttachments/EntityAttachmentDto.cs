using System;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.EntityAttachments;

public class EntityAttachmentDto : EntityDto<Guid>
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public FileAttachmentDto Attachment { get; set; } = default!;
}
