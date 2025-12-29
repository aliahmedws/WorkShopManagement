using System;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.FileAttachments;

namespace WorkShopManagement.CarModels;

public class CarModelDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public FileAttachmentDto FileAttachments { get; set; } = default!;
}
