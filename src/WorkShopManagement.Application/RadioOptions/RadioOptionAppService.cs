using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.RadioOptions;

[RemoteService(IsEnabled = false)]
[Authorize(WorkShopManagementPermissions.RadioOptions.Delete)]
public class RadioOptionAppService : ApplicationService, IRadioOptionAppService
{
    private readonly IRepository<RadioOption, Guid> _repository;

    public RadioOptionAppService(IRepository<RadioOption, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<List<RadioOptionDto>> GetListAsync(GetRadioOptionListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();

        var items = await AsyncExecuter.ToListAsync(queryable.Where(x => x.ListItemId == input.ListItemId));

        return ObjectMapper.Map<List<RadioOption>, List<RadioOptionDto>>(items);
    }

    [Authorize(WorkShopManagementPermissions.RadioOptions.Create)]
    public async Task<RadioOptionDto> CreateAsync(CreateRadioOptionDto input)
    {
        var name = input.Name?.Trim();

        var exists = await _repository.AnyAsync(x =>
            x.ListItemId == input.ListItemId &&
            x.Name == name);

        if (exists)
        {
            throw new UserFriendlyException($"Radio option '{name}' already exists for this list item.");
        }

        var entity = new RadioOption(GuidGenerator.Create(), input.ListItemId, name!);
        await _repository.InsertAsync(entity, autoSave: true);

        return ObjectMapper.Map<RadioOption, RadioOptionDto>(entity);
    }

    [Authorize(WorkShopManagementPermissions.RadioOptions.Edit)]
    public async Task<RadioOptionDto> UpdateAsync(Guid id, UpdateRadioOptionDto input)
    {
        var entity = await _repository.GetAsync(id);
        var name = input.Name?.Trim();

        var exists = await _repository.AnyAsync(x =>
            x.Id != id &&
            x.ListItemId == entity.ListItemId &&
            x.Name == name);

        if (exists)
        {
            throw new UserFriendlyException($"Radio option '{name}' already exists for this list item.");
        }

        if (entity.ConcurrencyStamp.IsNullOrWhiteSpace())
        {
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
        }

        entity.ChangeName(name!);
        await _repository.UpdateAsync(entity, autoSave: true);

        return ObjectMapper.Map<RadioOption, RadioOptionDto>(entity);
    }

    [Authorize(WorkShopManagementPermissions.RadioOptions.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}
