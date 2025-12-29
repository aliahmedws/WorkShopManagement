using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.CarModels;

public class GetCarModelListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public string? Name { get; set; }
}
