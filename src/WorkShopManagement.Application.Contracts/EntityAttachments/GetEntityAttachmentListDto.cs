using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.EntityAttachments;

public class GetEntityAttachmentListDto
{
    [Required]
    public Guid EntityId { get; set; }
    [Required]
    public EntityType EntityType { get; set; }
}
