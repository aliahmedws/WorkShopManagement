using System.Collections.Generic;

namespace WorkShopManagement.CarBayItems.BatchCarBayItems;

public class SaveCarBayItemBatchResultDto
{
    public List<CarBayItemDto> Items { get; set; } = new();
}
