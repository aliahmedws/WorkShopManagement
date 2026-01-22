using System;
using System.Collections.Generic;
using System.Text;

namespace WorkShopManagement.Recalls
{
    public class ExternalRecallDetailDto
    {
        public string Title { get; set; } = default!;           //Maps with RecallName
        public string? Make { get; set; }
        public string? ManufacturerId { get; set; }
        public string? RiskDescription { get; set; }
    }
}
