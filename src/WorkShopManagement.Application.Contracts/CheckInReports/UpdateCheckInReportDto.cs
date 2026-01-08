using System;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.Cars.StorageLocations;
using WorkShopManagement.Utils.Enums;

namespace WorkShopManagement.CheckInReports
{
    public class UpdateCheckInReportDto : EntityDto<Guid>
    {
        public ChoiceOptions? AvcStickerCut { get; set; }
        public ChoiceOptions? AvcStickerPrinted { get; set; }
        public int? BuildYear { get; set; }                 // from Car
        public int? BuildMonth { get; set; }                // from Car?
        public DateTime? ComplianceDate { get; set; }
        public ChoiceOptions? CompliancePlatePrinted { get; set; }
        public string? Emission { get; set; }
        public string? EngineNumber { get; set; }
        public int? EntryKms { get; set; }
        public double? FrontGwar { get; set; }
        public string? FrontMoterNumber { get; set; }
        public double? RearGwar { get; set; }
        public string? RearMotorNumber { get; set; }
        public double? MaxTowingCapacity { get; set; }
        public string? TyreLabel { get; set; }
        public string? RsvaImportApproval { get; set; }
        public string? ReportStatus { get; set; }

        public StorageLocation? StorageLocation { get; set; }
    }
}
