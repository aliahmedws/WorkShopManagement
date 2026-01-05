using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.ModelCategories;

public interface IModelCategoryAppService : IApplicationService
{
    Task<PagedResultDto<ModelCategoryDto>> GetListAsync(GetModelCategoryListDto input);
}
