using System;
using System.Collections.Generic;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.CarBayItems.BatchCarBayItems;

public class SaveCarBayItemRowDto
{
    public Guid? Id { get; set; }                      // null => create, not null => update
    public Guid CheckListItemId { get; set; }
    public Guid CarBayId { get; set; }
    public string? CheckRadioOption { get; set; }
    public string? Comments { get; set; }

    public List<FileAttachmentDto>? TempFiles { get; set; }
    public List<EntityAttachmentDto>? EntityAttachments { get; set; }

    public string? ConcurrencyStamp { get; set; }
}
