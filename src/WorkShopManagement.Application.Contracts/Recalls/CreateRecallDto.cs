using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.Recalls
{
    public class CreateRecallDto
    {
        [Required]
        public Guid CarId { get; set; }
        [Required]
        [StringLength(RecallConsts.MaxTitleLength)]
        public string Title { get; set; } = default!;
        [StringLength(RecallConsts.MaxMakeLength)]
        public string? Make { get; set; }
        [StringLength(RecallConsts.MaxManufactureIdLength)]
        public string? ManufactureId { get; set; }
        [StringLength(RecallConsts.MaxRiskDescriptionLength)]
        public string? RiskDescription { get; set; }
        public RecallType Type { get; set; } = RecallType.Recalls;
        public RecallStatus Status { get; set; } = RecallStatus.Pending;
        [StringLength(RecallConsts.MaxNotesLength)]
        public string? Notes { get; set; }

        public List<FileAttachmentDto> TempFiles { get; set; } = [];
    }
}
