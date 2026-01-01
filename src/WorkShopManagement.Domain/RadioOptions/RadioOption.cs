using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.ListItems;

namespace WorkShopManagement.RadioOptions;

[Audited]
public class RadioOption : FullAuditedAggregateRoot<Guid>
{
    public Guid ListItemId { get; set; }
    public virtual ListItem ListItems { get; private set; } = default!;

    public string Name { get; private set; } = default!;

    private RadioOption() { }

    public RadioOption(Guid id, Guid listItemId, string name)
        : base(id)
    {
        ListItemId = listItemId;
        SetName(name);
    }

    public void ChangeName(string name) => SetName(name);

    private void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: RadioOptionConsts.MaxNameLength).Trim();
    }
}
