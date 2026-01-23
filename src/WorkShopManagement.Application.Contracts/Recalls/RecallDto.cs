using System;
using System.Collections.Generic;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.Recalls
{
    public class RecallDto 
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Make { get; set; }
        public string? ManufactureId { get; set; }
        public string? RiskDescription { get; set; }
        public RecallType Type { get; set; }
        public RecallStatus Status { get; set; }
        public string? Notes { get; set; }
        public bool IsExternal { get; set; } = false; 
        public Guid CarId { get; set; }
        public List<FileAttachmentDto> TempFiles { get; set; } = [];
        public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
        public string? ConcurrencyStamp { get; set; } = default!;
    }
}
