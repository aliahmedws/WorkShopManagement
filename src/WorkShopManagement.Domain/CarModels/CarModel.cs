using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WorkShopManagement.CheckLists;
using WorkShopManagement.FileAttachments;

namespace WorkShopManagement.CarModels;

public class CarModel : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public FileAttachment FileAttachments { get; set; }
    public ICollection<CheckList> CheckLists { get; set; }

    private CarModel() 
    {
        CheckLists = new List<CheckList>();
    }

    internal CarModel(
        Guid id,
        string name,
        FileAttachment fileAttachments
    ) : base(id)
    {
        Name = name;
        FileAttachments = fileAttachments;
    }

}
