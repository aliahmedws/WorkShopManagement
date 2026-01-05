using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.ModelCategories;

[RemoteService(isEnabled: false)]
public class ModelCategoryAppService : ApplicationService, IModelCategoryAppService
{
    private readonly IRepository<ModelCategory, Guid> _repository;

    public ModelCategoryAppService(IRepository<ModelCategory, Guid> repository)
    {
        _repository = repository;
    }


    public async Task<PagedResultDto<ModelCategoryDto>> GetListAsync(GetModelCategoryListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();

        if (!input.Filter.IsNullOrWhiteSpace())
        {
            var f = input.Filter!.Trim();
            queryable = queryable.Where(x => x.Name.Contains(f));
        }

        var sorting = input.Sorting.IsNullOrWhiteSpace()
            ? nameof(ModelCategory.CreationTime)
            : input.Sorting;

        queryable = queryable.OrderBy(sorting);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var items = await AsyncExecuter.ToListAsync(
            queryable
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        var dtos = ObjectMapper.Map<List<ModelCategory>, List<ModelCategoryDto>>(items);
        return new PagedResultDto<ModelCategoryDto>(totalCount, dtos);
    }
}
