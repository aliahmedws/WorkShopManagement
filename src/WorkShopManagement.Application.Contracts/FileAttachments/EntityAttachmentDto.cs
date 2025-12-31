using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.FileAttachments;

public class EntityAttachmentDto : EntityDto<Guid>
{
    public Guid? CheckListId { get; set; }
    public Guid? ListItemId { get; set; }

    public FileAttachmentDto Attachment { get; set; } = default!;
}
