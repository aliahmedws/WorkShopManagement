using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.CarBayItems;

public class UpdateCarBayItemDto
{
    [Required]
    public Guid CheckListItemId { get; set; }

    [Required]
    public Guid CarBayId { get; set; }

    [MaxLength(CarBayItemConsts.MaxCheckRadioOptionLength)]
    public string? CheckRadioOption { get; set; }

    [MaxLength(CarBayItemConsts.MaxCommentsLength)]
    public string? Comments { get; set; }

    [Required]
    public string ConcurrencyStamp { get; set; } = default!;
    public List<FileAttachmentDto> TempFiles { get; set; } = [];
    public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
}
