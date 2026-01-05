using System;
using System.Collections.Generic;
using System.Text;

namespace WorkShopManagement.CheckInReports
{
    public class CheckInReportFiltersInput
    {
        public string? Sorting { get; set; }
        public int SkipCount { get; set; } = 0;
        public int MaxResultCount { get; set; } = 10;
        public string? Filter {  get; set; }
        public string? VinNo { get; set; }
        public string? Status { get; set; }
        public string? Model { get; set; }
        public string? StorageLocation { get; set; }
        public DateTime? BuildDateMin { get; set; }
        public DateTime? BuildDateMax { get; set; }
        public DateTime? ComplianceDateMin { get; set; }
        public DateTime? ComplianceDateMax { get; set; }
        public int? EntryKmsMin { get; set; }
        public int? EntryKmsMax { get; set; }
        public bool? AvcStickerCut { get; set; }
        public bool? CompliancePlatePrinted { get; set; }
    }
}
