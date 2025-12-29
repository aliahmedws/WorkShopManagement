using System;
using System.Collections.Generic;
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


    public async Task<PagedResultDto<CarModelDto>> GetListAsync()
    {

        var items = await _repository.GetListAsync();

        var totalCount = await _repository.GetCountAsync();

        var dtos = ObjectMapper.Map<List<CarModel>, List<CarModelDto>>(items);

        return new PagedResultDto<CarModelDto>(totalCount, dtos);
    }
}
