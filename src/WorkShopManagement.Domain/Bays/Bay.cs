using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkShopManagement.Bays;

public class Bay : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
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
