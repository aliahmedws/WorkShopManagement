using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.EntityAttachments;

namespace WorkShopManagement.LogisticsDetails.ArrivalEstimates
{
    public class ArrivalEstimateDto : FullAuditedEntityDto<Guid>
    {
        public Guid LogisticsDetailId { get; set; }
        public DateTime EtaPort { get; set; }
        public DateTime EtaScd { get; set; }
        public string? Notes { get; set; }

        public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
    }
}
