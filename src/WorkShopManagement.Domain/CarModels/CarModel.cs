using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.CheckLists;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.ModelCategories;

namespace WorkShopManagement.CarModels;

[Audited]
public class CarModel : FullAuditedAggregateRoot<Guid> //Variants
{
    public string Name { get; set; }
    public Guid ModelCategoryId { get; set; }
    public virtual ModelCategory ModelCategory { get; set; } = default!;
    public FileAttachment FileAttachments { get; set; }
    public ICollection<CheckList> CheckLists { get; set; }

    private CarModel()
    {
        CheckLists = new List<CheckList>();
    }

    internal CarModel(
        Guid id,
        Guid modelCategoryId,
        string name,
        FileAttachment fileAttachments
    ) : base(id)
    {
        ModelCategoryId = modelCategoryId;
        Name = name;
        FileAttachments = fileAttachments;
    }

}
