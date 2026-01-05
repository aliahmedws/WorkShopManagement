using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.CarModels;

public class GetCarModelListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? ModelCategoryId { get; set; }
    public Guid? CarModelId { get; set; }
}
