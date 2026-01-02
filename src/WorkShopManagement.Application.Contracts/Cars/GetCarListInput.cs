using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.Cars;

public class GetCarListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
