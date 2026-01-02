using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace WorkShopManagement.Cars;

[Audited]
public class CarOwner : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public string? Email { get; private set; }
    public string? ContactId { get; private set; }

    private CarOwner() { }

    public CarOwner(Guid id, string name, string? email, string? contactId) : base(id)
    {
        SetName(name);
        SetEmail(email);
        SetContactId(contactId);
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: CarOwnerConsts.MaxNameLength);
    }

    public void SetEmail(string? email)
    {
        Email = DomainCheck.TrimOptional(email, nameof(email), maxLength: CarOwnerConsts.MaxEmailLength);
    }

    public void SetContactId(string? contactId)
    {
        ContactId = DomainCheck.TrimOptional(contactId, nameof(contactId), maxLength: CarOwnerConsts.MaxContactIdLength);
    }

    public void Update(string name, string? email, string? contactId)
    {
        SetName(name);
        SetEmail(email);
        SetContactId(contactId);
    }
}
