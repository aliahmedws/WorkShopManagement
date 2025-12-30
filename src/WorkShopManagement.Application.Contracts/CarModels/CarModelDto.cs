using System;
using WorkShopManagement.FileAttachments;

namespace WorkShopManagement.CarModels;

public class CarModelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public FileAttachmentDto FileAttachments { get; set; } = default!;
}
