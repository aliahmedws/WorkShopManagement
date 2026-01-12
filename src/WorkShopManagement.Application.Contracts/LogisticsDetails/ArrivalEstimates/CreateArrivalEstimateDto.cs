using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.LogisticsDetails.ArrivalEstimates
{
    public class CreateArrivalEstimateDto
    {
        [Required]
        public Guid LogisticsDetailId { get; set; }

        [Required]
        public DateTime EtaPort { get; set; }

        [Required]
        public DateTime EtaScd { get; set; }

        [StringLength(ArrivalEstimateConsts.MaxNotesLength)]
        public string? Notes { get; set; }
        public List<FileAttachmentDto> TempFiles { get; set; } = [];
    }
}
