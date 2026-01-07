using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.CarModels;

[RemoteService(isEnabled: false)]
public class CarModelAppService : ApplicationService, ICarModelAppService
{
    private readonly IRepository<CarModel, Guid> _repository;

    public CarModelAppService(IRepository<CarModel, Guid> repository)
    {
        _repository = repository;
    }


    public async Task<PagedResultDto<CarModelDto>> GetListAsync(GetCarModelListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();

        if(!input.Filter.IsNullOrWhiteSpace())
        {
            var f = input.Filter.Trim();
            queryable = queryable.Where(x => x.Name.ToLower().Contains(f));
        }

        if (input.ModelCategoryId.HasValue)
        {
            queryable = queryable.Where(x => x.ModelCategoryId == input.ModelCategoryId.Value);
        }

        if(input.CarModelId.HasValue)
        {
            queryable = queryable.Where(x => x.Id == input.CarModelId.Value);
        }

        var sorting = input.Sorting.IsNullOrWhiteSpace()
            ? nameof(CarModel.CreationTime)
            : input.Sorting;

        queryable = queryable.OrderBy(sorting);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var items = await AsyncExecuter.ToListAsync(
            queryable
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        var dtos = ObjectMapper.Map<List<CarModel>, List<CarModelDto>>(items);
        return new PagedResultDto<CarModelDto>(totalCount, dtos);
    }
 
}
