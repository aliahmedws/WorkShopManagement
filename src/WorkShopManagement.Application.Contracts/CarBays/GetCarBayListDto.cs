using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.CarBays;

public class GetCarBayListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? CarId { get; set; }
    public Guid? BayId { get; set; }
    public bool? IsActive { get; set; }
}
