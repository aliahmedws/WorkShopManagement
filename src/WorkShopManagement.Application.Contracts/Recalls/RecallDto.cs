using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using WorkShopManagement.EntityAttachments;

namespace WorkShopManagement.Recalls
{
    public class RecallDto : FullAuditedEntityDto<Guid>
    {
        public string Title { get; set; } = default!;
        public string? Make { get; set; }
        public string? ManufactureId { get; set; }
        public string? RiskDescription { get; set; }
        public RecallType Type { get; set; }
        public RecallStatus Status { get; set; }
        public string? Notes { get; set; }
       
        public Guid CarId { get; set; }
        public string Vin { get; set; } = default!;     // TODO: implement include logic later
        public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
        public string? ConcurrencyStamp { get; set; } = default!;
    }
}
