using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.ModelCategories;

public class GetModelCategoryListDto : PagedAndSortedResultRequestDto
{
    public string? Filters { get; set; }
}
