using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WorkShopManagement.FileAttachments;

namespace WorkShopManagement.CarModels;

public class CarModel : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public FileAttachment FileAttachments { get; set; }

    private CarModel() { }

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
