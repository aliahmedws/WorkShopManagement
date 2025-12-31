using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.ListItems;

public class GetListItemListDto : PagedAndSortedResultRequestDto
{
    public Guid? CheckListId { get; set; }
    public string? Filter { get; set; }
}
