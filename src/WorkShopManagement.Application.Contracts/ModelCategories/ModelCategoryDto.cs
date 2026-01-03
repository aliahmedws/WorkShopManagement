using System;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.ModelCategories;

public class ModelCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public FileAttachmentDto FileAttachments { get; set; } = default!;
}
