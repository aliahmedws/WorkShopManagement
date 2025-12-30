using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.CheckLists;

[RemoteService(IsEnabled = false)]
[Authorize(WorkShopManagementPermissions.CheckLists.Default)]
public class CheckListAppService : ApplicationService, ICheckListAppService
{
    private readonly IRepository<CheckList, Guid> _checkListRepository;

    public CheckListAppService(IRepository<CheckList, Guid> checkListRepository)
    {
        _checkListRepository = checkListRepository;
    }

    public async Task<PagedResultDto<CheckListDto>> GetListAsync(GetCheckListListDto input)
    {
        var queryable = await _checkListRepository.GetQueryableAsync();

        if (input.CarModelId.HasValue)
        {
            queryable = queryable.Where(x => x.CarModelId == input.CarModelId.Value);
        }

        var sorting = input.Sorting.IsNullOrWhiteSpace()
            ? $"{nameof(CheckList.Position)} asc, {nameof(CheckList.Name)} asc"
            : input.Sorting;

        queryable = queryable.OrderBy(sorting);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var items = await AsyncExecuter.ToListAsync(
            queryable
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        var dtos = ObjectMapper.Map<List<CheckList>, List<CheckListDto>>(items);
        return new PagedResultDto<CheckListDto>(totalCount, dtos);
    }

    public async Task<CheckListDto> GetAsync(Guid id)
    {
        var entity = await _checkListRepository.GetAsync(id);
        return ObjectMapper.Map<CheckList, CheckListDto>(entity);
    }


    [Authorize(WorkShopManagementPermissions.CheckLists.Create)]
    public async Task<CheckListDto> CreateAsync(CreateCheckListDto input)
    {
        var name = input.Name?.Trim();

        var exists = await _checkListRepository.AnyAsync(x =>
            x.CarModelId == input.CarModelId &&
            x.Name == name);

        if (exists)
        {
            throw new UserFriendlyException($"Checklist '{name}' already exists for this car model.");
        }

        var positionExists = await _checkListRepository.AnyAsync(x =>
           x.CarModelId == input.CarModelId &&
           x.Position == input.Position);

        if (positionExists)
        {
            throw new UserFriendlyException($"Position '{input.Position}' already exists for this car model.");
        }

        var entity = new CheckList(
            GuidGenerator.Create(),
            name!,
            input.Position,
            input.CarModelId,
            input.CheckListType
        );

        await _checkListRepository.InsertAsync(entity, autoSave: true);

        return ObjectMapper.Map<CheckList, CheckListDto>(entity);
    }


    [Authorize(WorkShopManagementPermissions.CheckLists.Edit)]
    public async Task<CheckListDto> UpdateAsync(Guid id, UpdateCheckListDto input)
    {
        var entity = await _checkListRepository.GetAsync(id);

        var name = input.Name?.Trim();

        var nameExists = await _checkListRepository.AnyAsync(x =>
            x.Id != id &&
            x.CarModelId == input.CarModelId &&
            x.Name == name
        );

        if (nameExists)
        {
            throw new UserFriendlyException($"Checklist '{name}' already exists for this car model.");
        }

        var positionExists = await _checkListRepository.AnyAsync(x =>
            x.Id != id &&
            x.CarModelId == input.CarModelId &&
            x.Position == input.Position);

        if (positionExists)
        {
            throw new UserFriendlyException($"Position '{input.Position}' already exists for this car model.");
        }


        if (!input.ConcurrencyStamp.IsNullOrWhiteSpace())
        {
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
        }

        entity.CarModelId = input.CarModelId;
        entity.CheckListType = input.CheckListType;
        entity.ChangeName(name!);
        entity.ChangePosition(input.Position);

        await _checkListRepository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<CheckList, CheckListDto>(entity);
    }

    [Authorize(WorkShopManagementPermissions.CheckLists.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _checkListRepository.DeleteAsync(id);
    }
}
