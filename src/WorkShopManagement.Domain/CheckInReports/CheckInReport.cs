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
    public string? FrontGawr { get; set; }
    public string? FrontMotorNumber { get; set; }
    public string? RearGawr { get; set; }
    public string? RearMotorNumber { get; set; }
    public string? MaxTowingCapacity { get; set; }
    public string? TyreLabel { get; set; }
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
        string? frontGawr = null,
        string? rearGawr = null,
        string? frontMotorNumber = null,
        string? rearMotorNumber = null,
        string? maxTowingCapacity = null,
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
        SetGawrs(frontGawr, rearGawr);
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
            BuildYear = null;
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

    public void SetGawrs(string? frontGawr, string? rearGawr)
    {
        FrontGawr = DomainCheck.TrimOptional(frontGawr, nameof(FrontGawr), CheckInReportConsts.MaxLength);
        RearGawr = DomainCheck.TrimOptional(rearGawr, nameof(RearGawr), CheckInReportConsts.MaxLength);
    }
    public void SetMotorNumbers(string? frontMotorNumber, string? rearMotorNumber)
    {
        FrontMotorNumber = DomainCheck.TrimOptional(frontMotorNumber, nameof(FrontMotorNumber), CheckInReportConsts.MaxLength);
        RearMotorNumber = DomainCheck.TrimOptional(rearMotorNumber, nameof(RearMotorNumber), CheckInReportConsts.MaxLength);
    }


    public void SetSpecs(string? maxTowingCapacity, string? emission, string? tyreLabel)
    {
        MaxTowingCapacity = DomainCheck.TrimOptional(maxTowingCapacity, nameof(MaxTowingCapacity), CheckInReportConsts.MaxLength);
        Emission = DomainCheck.TrimOptional(emission, nameof(Emission), CheckInReportConsts.MaxLength);
        TyreLabel = DomainCheck.TrimOptional(tyreLabel, nameof(TyreLabel), CheckInReportConsts.MaxLength);
    }

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
        string? frontGawr = null,
        string? rearGawr = null,
        string? frontMotorNumber = null,
        string? rearMotorNumber = null,
        string? maxTowingCapacity = null,
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
        SetGawrs(frontGawr, rearGawr);
        SetMotorNumbers(frontMotorNumber, rearMotorNumber);
        SetSpecs(maxTowingCapacity, emission, tyreLabel);
        //SetRsva(rsvaImportApproval);
        SetStatus(reportStatus);
    }
}
