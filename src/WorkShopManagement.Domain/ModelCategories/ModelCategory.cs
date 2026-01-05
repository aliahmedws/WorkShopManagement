using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.CarModels;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.ModelCategories;

[Audited]
public class ModelCategory : FullAuditedAggregateRoot<Guid> //Vehicles
{
    public string Name { get; set; }
    public FileAttachment FileAttachments { get; set; }
    public ICollection<CarModel> CarModels { get; set; }

    private ModelCategory()
    {
        CarModels = new List<CarModel>();
    }

    internal ModelCategory(
        Guid id,
        string name,
        FileAttachment fileAttachments
    ) : base(id)
    {
        Name = name;
        FileAttachments = fileAttachments;
    }
}
