using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using WorkShopManagement.TempFiles;

namespace WorkShopManagement.EntityAttachments
{
    public class UpdateEntityAttachmentDto 
        //: IHasConcurrencyStamp
    {
        [Required]
        public Guid EntityId { get; set; }
        [Required]
        public EntityType EntityType { get; set; }
        [Required]
        public List<TempFileDto> TempFiles { get; set; } = [];
        [Required]
        public List<EntityAttachmentDto> Attachments { get; set; } = [];
        //[Required]
        //public string ConcurrencyStamp { get; set; } = default!;
    }
}
