using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.QualityGates;

public class QualityGateDto : FullAuditedEntityDto<Guid>
{
    public GateName GateName { get; set; }
    public QualityGateStatus Status { get; set; }
    public Guid CarBayId { get; set; }
    public string? ConcurrencyStamp { get; set; }
}
