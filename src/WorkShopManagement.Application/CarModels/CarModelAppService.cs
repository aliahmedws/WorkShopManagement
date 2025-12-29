using Microsoft.AspNetCore.Http;
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
    private readonly CarModelManager _manager;
    private readonly ICarModelRepository _repository;

    public CarModelAppService(CarModelManager manager, ICarModelRepository repository)
    {
        _manager = manager;
        _repository = repository;
    }

    public async Task<CarModelDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id, includeDetails: true);
        if (entity == null)
        {
            throw new CarModelNotFoundException();
        }

        return ObjectMapper.Map<CarModel, CarModelDto>(entity);
    }

    public async Task<PagedResultDto<CarModelDto>> GetListAsync(GetCarModelListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(CarModel.Name);
        }

        var items = await _repository.GetListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting!,
            input.Filter,
            input.Name,
            includeDetails: true
        );

        var totalCount = await _repository.GetCountAsync(
            input.Filter,
            input.Name
        );

        var dtos = ObjectMapper.Map<List<CarModel>, List<CarModelDto>>(items);

        return new PagedResultDto<CarModelDto>(totalCount, dtos);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _manager.DeleteAsync(id);
    }

    public async Task<CarModelDto> UpdateAsync(Guid id, UpdateCarModelDto input)
    {
        var entity = await _manager.UpdateAsync(
            id,
            input.Name,
            input.Description,
            input.ConcurrencyStamp
        );

        return ObjectMapper.Map<CarModel, CarModelDto>(entity);
    }

    public async Task<CarModelDto> UploadAsync(IFormFile file, CreateCarModelDto input)
    {
        var entity = await _manager.CreateAsync(
            input.Name,
            input.Description,
            file
        );

        return ObjectMapper.Map<CarModel, CarModelDto>(entity);
    }
}
