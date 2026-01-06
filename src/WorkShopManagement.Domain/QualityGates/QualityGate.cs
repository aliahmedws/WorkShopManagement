using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace WorkShopManagement.QualityGates;

[Audited]
public class QualityGate : FullAuditedAggregateRoot<Guid>
{
    public GateName GateName { get; set; }
    public QualityGateStatus Status { get; set; }

    //public Guid CarBayId { get; set; }

    private QualityGate() { }

    public QualityGate(
        Guid id,
        GateName gateName,
        QualityGateStatus status) : base(id)
    {
        GateName = gateName;
        Status = status;
    }
}
