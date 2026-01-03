using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.CarModels;

public class GetCarModelListDto : PagedAndSortedResultRequestDto
{
    public string? Filters { get; set; }
    public Guid? ModelCategoryId { get; set; }
}
