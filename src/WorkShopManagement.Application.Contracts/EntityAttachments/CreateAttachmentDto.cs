using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.TempFiles;

namespace WorkShopManagement.EntityAttachments
{
    public class CreateAttachmentDto
    {
        [Required]
        public Guid EntityId { get; set; }
        [Required]
        public EntityType EntityType { get; set; }
        [Required]
        public List<TempFileDto> TempFiles { get; set; } = [];

    }
}
