using System;
using System.Collections.Generic;

namespace WorkShopManagement.CarBayItems.BatchCarBayItems;

public class SaveCarBayItemBatchDto
{
    public Guid CarBayId { get; set; }
    public List<SaveCarBayItemRowDto> Items { get; set; } = new();
}
