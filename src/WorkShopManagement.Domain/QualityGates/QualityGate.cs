using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.Bays;
using WorkShopManagement.CarBays;

namespace WorkShopManagement.QualityGates;

[Audited]
public class QualityGate : FullAuditedAggregateRoot<Guid>
{
    public GateName GateName { get; set; }
    public QualityGateStatus Status { get; set; }

    //public Guid CarBayId { get; set; }

    public virtual ICollection<CarBay?> CarBays { get; set; } = default!;

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
