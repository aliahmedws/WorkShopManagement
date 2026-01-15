using System;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.Cars;
using WorkShopManagement.Cars.StorageLocations;
using WorkShopManagement.Utils.Enums;

namespace WorkShopManagement.CheckInReports
{
    public class CheckInReportDto : FullAuditedEntityDto<Guid>
    {

        public int? BuildYear { get; set; }     
        public int? BuildMonth { get; set; }
        public ChoiceOptions? AvcStickerCut { get; set; }
        public ChoiceOptions? AvcStickerPrinted { get; set; }
        public ChoiceOptions? CompliancePlatePrinted { get; set; }
        public DateTime? ComplianceDate { get; set; }
        public int? EntryKms { get; set; }
        public string? EngineNumber { get; set; }
        public double? FrontGwar { get; set; }
        public double? RearGwar { get; set; }
        public string? FrontMoterNumber { get; set; }
        public string? RearMotorNumber { get; set; }
        public double? MaxTowingCapacity { get; set; }
        public string? Emission { get; set; }
        public string? TyreLabel { get; set; }
        //public string? RsvaImportApproval { get; set; }
        public string? ReportStatus { get; set; }
        public string? ConcurrencyStamp { get; set; }
        //public string? CreatorName { get; set; }            // From Audit

        public Guid CarId { get; set; }
        // Car Details
        //public string VinNo { get; set; } = default!;
        public StorageLocation? StorageLocation { get; set; }

        // Car - CarModel Details
        //public string ModelName { get; set; } = default!;

        public CarDto? Car { get; set; } = default!;


    }
}
