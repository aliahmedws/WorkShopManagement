using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.EntityAttachments;

public class CreateAttachmentDto
{
    [Required]
    public Guid EntityId { get; set; }

    [Required]
    public EntityType EntityType { get; set; }

    public EntitySubType? SubType { get; set; }

    [Required]
    public List<FileAttachmentDto> TempFiles { get; set; } = [];
}