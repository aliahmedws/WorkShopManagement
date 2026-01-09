using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.Cars;
using WorkShopManagement.Utils.Enums;

namespace WorkShopManagement.CheckInReports;

[Audited]
public class CheckInReport : FullAuditedAggregateRoot<Guid>
{

    public int? BuildYear { get; set; }                 // from Car
    public int? BuildMonth { get; set; }                // from Car?
    public ChoiceOptions? AvcStickerCut { get; set; }
    public ChoiceOptions? AvcStickerPrinted { get; set; }
    public ChoiceOptions? CompliancePlatePrinted { get; set; }
    public DateTime? ComplianceDate { get; set; }
    public string? Emission { get; set; }
    public string? EngineNumber { get; set; }
    public int? EntryKms { get; set; }
    public double? FrontGwar { get; set; }
    public string? FrontMoterNumber { get; set; }
    public double? RearGwar { get; set; }
    public string? RearMotorNumber { get; set; }
    public double? MaxTowingCapacity { get; set; }
    public string? TyreLabel { get; set; }
    //public string? RsvaImportApproval { get; set; }         //TODO: move it to car
    public string? ReportStatus { get; set; }

    public Guid CarId { get; set; }
    public virtual Car? Car { get; set; }

    private CheckInReport()
    {
    }

    // Required invariants
    public CheckInReport(
        Guid id,
        Guid carId,
        int? buildYear = null,
        int? buildMonth = null,
        ChoiceOptions? avcStickerCut = null,
        ChoiceOptions? avcStickerPrinted = null,
        ChoiceOptions? compliancePlatePrinted = null,
        DateTime? complianceDate = null,
        int? entryKms = null,
        string? engineNumber = null,
        double? frontGwar = null,
        double? rearGwar = null,
        string? frontMotorNumber = null,
        string? rearMotorNumber = null,
        double? maxTowingCapacity = null,
        string? emission = null,
        string? tyreLabel = null,
        //string? rsvaImportApproval = null,
        string? reportStatus = null
    ) : base(id)
    {
        CarId = carId;
        SetBuildDate(buildYear, buildMonth);
        SetAvcSticker(avcStickerCut, avcStickerPrinted);
        SetCompliance(compliancePlatePrinted, complianceDate);
        SetEntryKms(entryKms);
        SetEngineNumber(engineNumber);
        SetGwars(frontGwar, rearGwar);
        SetMotorNumbers(frontMotorNumber,rearMotorNumber);
        SetSpecs(maxTowingCapacity, emission, tyreLabel);
        //SetRsva(rsvaImportApproval);
        SetStatus(reportStatus);

    }

    public void SetBuildDate(int? buildYear, int? buildMonth)
    {
        if (buildMonth.HasValue)
        {
            if (buildMonth < 1 || buildMonth > 12)
            {
                throw new BusinessException("CheckInReport:InvalidBuildMonth")
                    .WithData("BuildMonth", buildMonth.Value);
            }

            BuildMonth = buildMonth;
        }
        else
        {
            BuildMonth = null;
        }


        if (buildYear.HasValue)
        {
            if (buildYear < 1800 || buildYear > DateTime.Now.Year + 1)
            {
                throw new BusinessException("CheckInReport:InvalidBuildYear")
                    .WithData("BuildYear", buildYear.Value);
            }
            BuildYear = buildYear;
        }
        else
        {
            buildMonth = null;
        }

    }

    public void SetAvcSticker(ChoiceOptions? cut, ChoiceOptions? printed)
    {
        AvcStickerCut = cut;
        AvcStickerPrinted = printed;
    }

    public void SetCompliance(ChoiceOptions? platePrinted, DateTime? complianceDate)
    {

        ComplianceDate = complianceDate;
        CompliancePlatePrinted = platePrinted;
    }

    public void SetEntryKms(int? entryKms)
    {
        if (entryKms.HasValue && entryKms.Value < 0)
        {
            throw new BusinessException("CheckInReport:InvalidEntryKms")
                .WithData("EntryKms", entryKms.Value);
        }
        EntryKms = entryKms;
    }

    public void SetEngineNumber(string? engineNumber)
    {
        EngineNumber = DomainCheck.TrimOptional(engineNumber, nameof(EngineNumber), CheckInReportConsts.MaxLength);
    }

    public void SetGwars(double? frontGwar, double? rearGwar)
    {
        if (frontGwar.HasValue)
        {
            if (frontGwar < 0)
            {
                throw new BusinessException("CheckInReport:InvalidFrontGwar")
                    .WithData("FrontGwar", frontGwar.Value);
            }
            FrontGwar = frontGwar;
        }
        else
        {
            FrontGwar = null;
        }


        if (rearGwar.HasValue)
        {
            if (rearGwar < 0)
            {
                throw new BusinessException("CheckInReport:InvalidRearGwar")
                    .WithData("RearGwar", rearGwar.Value);
            }
            RearGwar = rearGwar;
        }
        else
        {
            RearGwar = null;
        }
    }
    public void SetMotorNumbers(string? frontMotorNumber, string? rearMotorNumber)
    {
        FrontMoterNumber = DomainCheck.TrimOptional(frontMotorNumber, nameof(FrontMoterNumber), CheckInReportConsts.MaxLength);
        RearMotorNumber = DomainCheck.TrimOptional(rearMotorNumber, nameof(RearMotorNumber), CheckInReportConsts.MaxLength);
    }


    public void SetSpecs(double? maxTowingCapacity, string? emission, string? tyreLabel)
    {
        if (maxTowingCapacity.HasValue && maxTowingCapacity < 0)
        {
            throw new BusinessException("CheckInReport:InvalidMaxTowingCapacity")
                .WithData("MaxTowingCapacity", maxTowingCapacity);
        }

        MaxTowingCapacity = maxTowingCapacity;
        Emission = DomainCheck.TrimOptional(emission, nameof(Emission), CheckInReportConsts.MaxLength);
        TyreLabel = DomainCheck.TrimOptional(tyreLabel, nameof(TyreLabel), CheckInReportConsts.MaxLength);
    }

    //public void SetRsva(string? rsvaImportApproval)
    //{
    //    RsvaImportApproval = DomainCheck.TrimOptional(rsvaImportApproval, nameof(RsvaImportApproval), CheckInReportConsts.MaxLength);
    //}

    public void SetStatus(string? status)
    {
        ReportStatus = DomainCheck.TrimOptional(status, nameof(ReportStatus), CheckInReportConsts.MaxLength);
    }


    public void UpdateCore(
        int? buildYear = null,
        int? buildMonth = null,
        ChoiceOptions? avcStickerCut = null,
        ChoiceOptions? avcStickerPrinted = null,
        DateTime? complianceDate = null,
        ChoiceOptions? compliancePlatePrinted = null,
        int? entryKms = null,
        string? engineNumber = null,
        double? frontGwar = null,
        double? rearGwar = null,
        string? frontMotorNumber = null,
        string? rearMotorNumber = null,
        double? maxTowingCapacity = null,
        string? emission = null,
        string? tyreLabel = null,
        //string? rsvaImportApproval = null,
        string? reportStatus = null
    )
    {
        SetBuildDate(buildYear, buildMonth);
        SetAvcSticker(avcStickerCut, avcStickerPrinted);
        SetCompliance(compliancePlatePrinted, complianceDate);
        SetEntryKms(entryKms);
        SetEngineNumber(engineNumber);
        SetGwars(frontGwar, rearGwar);
        SetMotorNumbers(frontMotorNumber, rearMotorNumber);
        SetSpecs(maxTowingCapacity, emission, tyreLabel);
        //SetRsva(rsvaImportApproval);
        SetStatus(reportStatus);
    }
}
