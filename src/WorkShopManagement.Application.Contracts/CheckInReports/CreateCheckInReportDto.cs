using System;
using System.Collections.Generic;
using System.Text;

namespace WorkShopManagement.CheckInReports
{
    public class CreateCheckInReportDto
    {
        public string VinNo { get; set; } = default!;
        public bool? AvcStickerCut { get; set; }
        public bool? AvcStickerPrinted { get; set; }
        public DateTime? BuildDate { get; set; }
        public string? CheckInSumbitterUser { get; set; }
        public DateTime? ComplianceDate { get; set; }
        public bool? CompliancePlatePrinted { get; set; }
        public string? Emission { get; set; }
        public string? EngineNumber { get; set; }
        public int? EntryKms { get; set; }
        public int? FrontGwar { get; set; }
        public string? FrontMoterNumbr { get; set; }
        public int? RearGwar { get; set; }
        public string? RearMotorNumber { get; set; }
        public string? HsObjectId { get; set; }
        public float MaxTowingCapacity { get; set; }
        public string? TyreLabel { get; set; }
        public string? RsvaImportApproval { get; set; }
        public string? Status { get; set; }
        public string? Model { get; set; }
        public string? StorageLocation { get; set; }
        public Guid CarId { get; set; }
    }
}
