using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.Recalls
{
    public class UpdateRecallDto : EntityDto<Guid>, IHasConcurrencyStamp
    {
        [Required]
        [StringLength(RecallConsts.MaxTitleLength)]
        public string Title { get; set; } = default!;
        //[StringLength(RecallConsts.MaxMakeLength)]
        //public string? Make { get; set; }
        //[StringLength(RecallConsts.MaxManufactureIdLength)]
        //public string? RiskDescription { get; set; }
        [Required]
        public RecallType Type { get; set; }
        [Required]
        public RecallStatus Status { get; set; }
        [StringLength(RecallConsts.MaxNotesLength)]
        public string? Notes { get; set; }
        [Required]
        public string ConcurrencyStamp { get; set; } = default!;

        public List<FileAttachmentDto> TempFiles { get; set; } = [];
        public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
    }
}
