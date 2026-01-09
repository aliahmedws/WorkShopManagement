using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.CarBays;

namespace WorkShopManagement.Bays;

[Audited]
public class Bay : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public virtual ICollection<CarBay> CarBays { get; set; } = default!;

    private Bay() { }

    internal Bay(Guid id, string name, bool isActive) : base(id)
    {
        Name = name;
        IsActive = isActive;
    }

    public Bay SetActive(bool isActive)
    {
        IsActive = isActive;
        return this;
    }
}
