using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.CheckLists;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.CarModels;

[Audited]
public class CarModel : FullAuditedAggregateRoot<Guid>
{
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
