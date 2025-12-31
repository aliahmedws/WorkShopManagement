using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace WorkShopManagement.Bays;

[Audited]
public class Bay : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; }

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
