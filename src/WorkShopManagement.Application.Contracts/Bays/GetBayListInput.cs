using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.Bays;

public class GetBayListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
