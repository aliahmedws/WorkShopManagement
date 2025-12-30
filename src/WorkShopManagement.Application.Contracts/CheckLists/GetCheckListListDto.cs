using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.CheckLists;

public class GetCheckListListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public string? Name { get; set; }
    public int? Position { get; set; }
    public CheckListType? CheckListType { get; set; }
    public Guid? CarModelId { get; set; }
}
