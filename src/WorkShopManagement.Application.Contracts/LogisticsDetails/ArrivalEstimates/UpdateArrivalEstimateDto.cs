using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.LogisticsDetails.ArrivalEstimates
{
    public class UpdateArrivalEstimateDto
    {
        [Required]

        [StringLength(ArrivalEstimateConsts.MaxNotesLength)]
        public string? Notes { get; set; }

        public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
        public List<FileAttachmentDto> TempFiles { get; set; } = [];
    }
}
