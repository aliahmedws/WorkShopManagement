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
    //public string? HsObjectId { get; set; }
    public double? MaxTowingCapacity { get; set; }
    public string? TyreLabel { get; set; }
    public string? RsvaImportApproval { get; set; }
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
        string? checkInSubmitterUser = null,
        ChoiceOptions? avcStickerCut = null,
        ChoiceOptions? avcStickerPrinted = null,
        DateTime? complianceDate = null,
        ChoiceOptions? compliancePlatePrinted = null,
        string? emission = null,
        string? engineNumber = null,
        int? entryKms = null,
        double? frontGwar = null,
        string? frontMotorNumber = null,
        double? rearGwar = null,
        string? rearMotorNumber = null,
        //string? hsObjectId = null,
        double? maxTowingCapacity = null,
        string? tyreLabel = null,
        string? rsvaImportApproval = null,
        string? status = null
    ) : base(id)
    {
        CarId = carId;
        SetBuildDate(buildYear, buildMonth);
        SetAvcSticker(avcStickerCut, avcStickerPrinted);
        SetCompliance(complianceDate, compliancePlatePrinted);
        SetMechanicalInfo(engineNumber, emission, entryKms);
        SetMotors(frontGwar, frontMotorNumber, rearGwar, rearMotorNumber);
        //SetHsInfo(hsObjectId);
        SetSpecs(maxTowingCapacity, tyreLabel);
        SetRsva(rsvaImportApproval);
        SetStatus(status);

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

    public void SetCompliance(DateTime? complianceDate, ChoiceOptions? platePrinted)
    {

        ComplianceDate = complianceDate;
        CompliancePlatePrinted = platePrinted;
    }

    public void SetMechanicalInfo(string? engineNumber, string? emission, int? entryKms)
    {
        EngineNumber = DomainCheck.TrimOptional(engineNumber, nameof(EngineNumber), CheckInReportConsts.MaxLength);
        Emission = DomainCheck.TrimOptional(emission, nameof(Emission), CheckInReportConsts.MaxLength);

        if (entryKms.HasValue && entryKms.Value < 0)
        {
            throw new BusinessException("CheckInReport:InvalidEntryKms")
                .WithData("EntryKms", entryKms.Value);
        }

        EntryKms = entryKms;
    }

    public void SetMotors(double? frontGwar, string? frontMotorNumber, double? rearGwar, string? rearMotorNumber)
    {

        if(frontGwar.HasValue)
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


        if(rearGwar.HasValue)
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


        FrontMoterNumber = DomainCheck.TrimOptional(frontMotorNumber, nameof(FrontMoterNumber), CheckInReportConsts.MaxLength);
        RearMotorNumber = DomainCheck.TrimOptional(rearMotorNumber, nameof(RearMotorNumber), CheckInReportConsts.MaxLength);
    }


    public void SetSpecs(double? maxTowingCapacity, string? tyreLabel)
    {
        if (maxTowingCapacity.HasValue && maxTowingCapacity < 0)
        {
            throw new BusinessException("CheckInReport:InvalidMaxTowingCapacity")
                .WithData("MaxTowingCapacity", maxTowingCapacity);
        }

        MaxTowingCapacity = maxTowingCapacity;
        TyreLabel = DomainCheck.TrimOptional(tyreLabel, nameof(TyreLabel), CheckInReportConsts.MaxLength);
    }

    public void SetRsva(string? rsvaImportApproval)
    {
        RsvaImportApproval = DomainCheck.TrimOptional(rsvaImportApproval, nameof(RsvaImportApproval), CheckInReportConsts.MaxLength);
    }

    public void SetStatus(string? status)
    {
        ReportStatus = DomainCheck.TrimOptional(status, nameof(ReportStatus), CheckInReportConsts.MaxLength);
    }


    public void UpdateCore(
        ChoiceOptions? avcStickerCut,
        ChoiceOptions? avcStickerPrinted,
        int? buildYear = null,
        int? buildMonth = null,
        string? checkInSubmitterUser = null,
        DateTime? complianceDate = null,
        ChoiceOptions? compliancePlatePrinted = null,
        string? emission = null,
        string? engineNumber = null,
        int? entryKms = null,
        int? frontGwar = null,
        string? frontMotorNumber = null,
        int? rearGwar = null,
        string? rearMotorNumber = null,
        float maxTowingCapacity = 0,
        string? tyreLabel = null,
        string? rsvaImportApproval = null,
        string? status = null
    )
    {
        SetAvcSticker(avcStickerCut, avcStickerPrinted);
        SetBuildDate(buildYear, buildMonth);
        SetCompliance(complianceDate, compliancePlatePrinted);
        SetMechanicalInfo(engineNumber, emission, entryKms);
        SetMotors(frontGwar, frontMotorNumber, rearGwar, rearMotorNumber);
        SetSpecs(maxTowingCapacity, tyreLabel);
        SetRsva(rsvaImportApproval);
        SetStatus(status);
    }
}
