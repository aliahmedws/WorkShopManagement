using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.EntityAttachments
{
    public class UpdateEntityAttachmentDto 
        //: IHasConcurrencyStamp
    {
        [Required]
        public Guid EntityId { get; set; }
        [Required]
        public EntityType EntityType { get; set; }

        public EntitySubType? SubType { get; set; }

        [Required]
        public List<FileAttachmentDto> TempFiles { get; set; } = [];
        [Required]
        public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
        //[Required]
        //public string ConcurrencyStamp { get; set; } = default!;
    }
}
