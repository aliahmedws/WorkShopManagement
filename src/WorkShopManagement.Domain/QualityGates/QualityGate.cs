using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.CarBays;

namespace WorkShopManagement.QualityGates;

[Audited]
public class QualityGate : FullAuditedAggregateRoot<Guid>
{
    public GateName GateName { get; set; }
    public QualityGateStatus Status { get; set; }
    public Guid CarBayId { get; set; }
    public virtual CarBay CarBays { get; set; }

    private QualityGate() { }

    public QualityGate(
        Guid id,
        Guid carBayId,
        GateName gateName,
        QualityGateStatus status) : base(id)
    {
        CarBayId = carBayId;
        GateName = gateName;
        Status = status;
    }
}
