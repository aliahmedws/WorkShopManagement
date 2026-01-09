using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.CarBayItems;

public class GetCarBayItemListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? CheckListItemId { get; set; }
    public Guid? CarBayId { get; set; }
}
