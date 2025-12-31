using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WorkShopManagement.CarModels;
using WorkShopManagement.ListItems;

namespace WorkShopManagement.CheckLists;

public class CheckList : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid CarModelId { get; set; }
    public virtual CarModel CarModels { get; set; }
    public string Name { get; set; } = default!;
    public int Position { get; set; }
    public CheckListType CheckListType { get; set; }
    public virtual ICollection<ListItem> ListItems { get; set; }

    private CheckList() 
    {
        ListItems = new List<ListItem>();
    }

    public CheckList(Guid id, string name, int position, Guid carModelId, CheckListType checkListType) : base(id)
    {
        CarModelId = carModelId;
        SetName(name);
        SetPosition(position);
        CheckListType = checkListType;
    }

    public CheckList ChangeName(string name)
    {
        SetName(name);
        return this;
    }

    public CheckList ChangePosition(int position)
    {
        SetPosition(position);
        return this;
    }

    private void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: CheckListConsts.NameMaxLength);
    }

    private void SetPosition(int position)
    {
        if (position < 0)
        {
            throw new UserFriendlyException("Invalid Value.");
        }

        Position = position;
    }
}
