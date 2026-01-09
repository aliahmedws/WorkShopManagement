using System;
using WorkShopManagement.Cars.StorageLocations;
using WorkShopManagement.Utils.Enums;

namespace WorkShopManagement.CheckInReports
{
    public class CheckInReportFiltersInput
    {
        public string? Sorting { get; set; }
        public int SkipCount { get; set; } = 0;
        public int MaxResultCount { get; set; } = 10;
        public string? Filter {  get; set; }
        public int? BuildYear { get; set; }
        public DateTime? ComplianceDateMin { get; set; }
        public DateTime? ComplianceDateMax { get; set; }
        public int? EntryKmsMin { get; set; }
        public int? EntryKmsMax { get; set; }
        public ChoiceOptions? AvcStickerCut { get; set; }
        public ChoiceOptions? CompliancePlatePrinted { get; set; }
        public string? ReportStatus { get; set; }
        public Guid? CreatorId { get; set; }            // From Audit
        // Car Filters
        public string? Vin { get; set; }
        public string? Model { get; set; }
        public StorageLocation? StorageLocation { get; set; }
    }
}
