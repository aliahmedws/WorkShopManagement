using Volo.Abp.Application.Dtos;
using WorkShopManagement.Stages;

namespace WorkShopManagement.Cars;

public class GetCarListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Stage? Stage { get; set; } 
}
