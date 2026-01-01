using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.Priorities;
public class GetPriorityListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
