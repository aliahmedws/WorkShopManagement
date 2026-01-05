using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.Cars;

namespace WorkShopManagement.CheckInReports;

public class CheckInReport : FullAuditedAggregateRoot<Guid>
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
    public float? MaxTowingCapacity { get; set; }
    public string? TyreLabel { get; set; }
    public string? RsvaImportApproval { get; set; }
    public string? Status { get; set; }
    public string? Model { get; set; }
    public string? StorageLocation { get; set; }
    public Guid CarId { get; set; }
    public virtual Car Car { get; set; } = default!;


    protected CheckInReport()
    {
    }

    // Required invariants
    public CheckInReport(
        Guid id,
        Guid carId,
        string vinNo,
        DateTime? buildDate,
        string? checkInSubmitterUser = null,
        bool? avcStickerCut = null,
        bool? avcStickerPrinted = null,
        DateTime? complianceDate = null,
        bool? compliancePlatePrinted = null,
        string? emission = null,
        string? engineNumber = null,
        int? entryKms = null,
        int? frontGwar = null,
        string? frontMotorNumber = null,
        int? rearGwar = null,
        string? rearMotorNumber = null,
        string? hsObjectId = null,
        float? maxTowingCapacity = null,
        string? tyreLabel = null,
        string? rsvaImportApproval = null,
        string? status = null,
        string? model = null,
        string? storageLocation = null
    ) : base(id)
    {
        SetCarId(carId);
        SetVinNo(vinNo);
        SetBuildDate(buildDate);
        SetCheckInSubmitter(checkInSubmitterUser);
        SetAvcSticker(avcStickerCut, avcStickerPrinted);
        SetCompliance(complianceDate, compliancePlatePrinted);
        SetMechanicalInfo(engineNumber, emission, entryKms);
        SetMotors(frontGwar, frontMotorNumber, rearGwar, rearMotorNumber);
        SetHsInfo(hsObjectId);
        SetSpecs(maxTowingCapacity, tyreLabel);
        SetRsva(rsvaImportApproval);
        SetVehicleIdentity(model, status);
        SetStorageLocation(storageLocation);

    }


    public void SetCarId(Guid carId)
    {
        if (carId == Guid.Empty)
        {
            throw new BusinessException("CheckInReport:CarIdRequired");
        }

        CarId = carId;
    }

    public void SetVinNo(string vinNo)
    {
        vinNo = (vinNo ?? string.Empty).Trim();
        Check.NotNullOrWhiteSpace(vinNo, nameof(vinNo));
        Check.Length(vinNo, nameof(vinNo), maxLength: CheckInReportConsts.VINMaxLength);

        VinNo = vinNo;
    }

    public void SetBuildDate(DateTime? buildDate)
    {
        BuildDate = buildDate;
    }

    public void SetCheckInSubmitter(string? user)
    {
        CheckInSumbitterUser = DomainCheck.TrimOptional(user, nameof(CheckInSumbitterUser), CheckInReportConsts.MaxLength);
    }

    public void SetAvcSticker(bool? cut, bool? printed)
    {
        AvcStickerCut = cut;
        AvcStickerPrinted = printed;
    }

    public void SetCompliance(DateTime? complianceDate, bool? platePrinted)
    {
        if (platePrinted == true && !complianceDate.HasValue)
        {
            throw new BusinessException("CheckInReport:ComplianceDateRequired");
        }

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

    public void SetMotors(int? frontGwar, string? frontMotorNumber, int? rearGwar, string? rearMotorNumber)
    {
        if (frontGwar.HasValue && frontGwar.Value < 0)
        {
            throw new BusinessException("CheckInReport:InvalidFrontGwar")
                .WithData("FrontGwar", frontGwar.Value);
        }

        if (rearGwar.HasValue && rearGwar.Value < 0)
        {
            throw new BusinessException("CheckInReport:InvalidRearGwar")
                .WithData("RearGwar", rearGwar.Value);
        }

        FrontGwar = frontGwar;
        RearGwar = rearGwar;

        FrontMoterNumbr = DomainCheck.TrimOptional(frontMotorNumber, nameof(FrontMoterNumbr), CheckInReportConsts.MaxLength);
        RearMotorNumber = DomainCheck.TrimOptional(rearMotorNumber, nameof(RearMotorNumber), CheckInReportConsts.MaxLength);
    }

    public void SetHsInfo(string? hsObjectId)
    {
        HsObjectId = DomainCheck.TrimOptional(hsObjectId, nameof(HsObjectId), CheckInReportConsts.MaxLength);
    }

    public void SetSpecs(float? maxTowingCapacity, string? tyreLabel)
    {
        if (maxTowingCapacity < 0)
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

    public void SetVehicleIdentity(string? model, string? status)
    {
        Model = DomainCheck.TrimOptional(model, nameof(Model), CheckInReportConsts.MaxLength);
        Status = DomainCheck.TrimOptional(status, nameof(Status), CheckInReportConsts.MaxLength);
    }

    public void SetStorageLocation(string? storageLocation)
    {
        StorageLocation = DomainCheck.TrimOptional(storageLocation, nameof(StorageLocation), CheckInReportConsts.MaxLength);
    }

    public void UpdateCore(
        bool avcStickerCut,
        bool avcStickerPrinted,
        DateTime buildDate,
        string? checkInSubmitterUser,
        DateTime? complianceDate,
        bool? compliancePlatePrinted,
        string? emission,
        string? engineNumber,
        int? entryKms,
        int? frontGwar,
        string? frontMotorNumber,
        int? rearGwar,
        string? rearMotorNumber,
        string? hsObjectId,
        float maxTowingCapacity,
        string? tyreLabel,
        string? rsvaImportApproval,
        string? status,
        string? model,
        string? storageLocation
    )
    {
        SetAvcSticker(avcStickerCut, avcStickerPrinted);
        SetBuildDate(buildDate);
        SetCheckInSubmitter(checkInSubmitterUser);
        SetCompliance(complianceDate, compliancePlatePrinted);
        SetMechanicalInfo(engineNumber, emission, entryKms);
        SetMotors(frontGwar, frontMotorNumber, rearGwar, rearMotorNumber);
        SetHsInfo(hsObjectId);
        SetSpecs(maxTowingCapacity, tyreLabel);
        SetRsva(rsvaImportApproval);
        SetVehicleIdentity(model, status);
        SetStorageLocation(storageLocation);
    }
}
