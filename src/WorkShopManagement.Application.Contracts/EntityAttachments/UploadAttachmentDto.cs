using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.EntityAttachments
{
    public class UploadAttachmentDto : EntityDto<Guid>
    {
        [Required]
        public Guid EntityId { get; set; }
        [Required]
        public EntityType EntityType { get; set; }

    }
}
