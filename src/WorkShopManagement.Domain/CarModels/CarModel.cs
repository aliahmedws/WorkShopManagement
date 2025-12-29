using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WorkShopManagement.FileAttachments;

namespace WorkShopManagement.CarModels;

public class CarModel : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public FileAttachment FileAttachments { get; set; }

    private CarModel() { }

    internal CarModel(
        Guid id,
        string name,
        string? description,
        FileAttachment fileAttachments
    ) : base(id)
    {
        SetName(name);
        SetDescription(description);
        SetFileAttachments(fileAttachments);
    }

    public CarModel ChangeName(string name)
    {
        SetName(name);
        return this;
    }

    public CarModel ChangeDescription(string? description)
    {
        SetDescription(description); return this;
    }

    public CarModel ChangeFileAttachments(FileAttachment fileAttachments)
    {
        SetFileAttachments(fileAttachments);
        return this;
    }

    private void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: CarModelConsts.NameMaxLength);
    }

    private void SetDescription(string? description)
    {
        if (description.IsNullOrWhiteSpace())
        {
            Description = null;
            return;
        }

        Description = Check.Length(
            description,
            nameof(description),
            CarModelConsts.DescriptionMaxLength,
            0
        );
    }

    private void SetFileAttachments(FileAttachment fileAttachments)
    {
        FileAttachments = Check.NotNull(fileAttachments, nameof(fileAttachments));
    }
}
