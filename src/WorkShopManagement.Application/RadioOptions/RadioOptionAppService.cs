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
[Authorize(WorkShopManagementPermissions.RadioOptions.Default)]
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

        var items = queryable
        .Where(x => x.ListItemId == input.ListItemId)
        .AsEnumerable()
        .OrderBy(x =>
        {
            var i = Array.IndexOf(
                RadioOptionConsts.OrderedNames,
                x.Name.ToUpperInvariant()
            );
            return i < 0 ? int.MaxValue : i;
        })
        .ThenBy(x => x.Name)
        .ToList();

        return ObjectMapper.Map<List<RadioOption>, List<RadioOptionDto>>(items);
    }

    [Authorize(WorkShopManagementPermissions.RadioOptions.Create)]
    public async Task<List<RadioOptionDto>> CreateAsync(CreateRadioOptionDto input)
    {
        if (input.Names == null || input.Names.Count == 0)
            return new List<RadioOptionDto>();

        var normalized = input.Names
            .Select(x => x?.Trim())
            .Where(x => !x.IsNullOrWhiteSpace())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalized.Count == 0)
            return new List<RadioOptionDto>();

        var queryable = await _repository.GetQueryableAsync();
        var existing = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.ListItemId == input.ListItemId)
            .Select(x => x.Name));

        var existingSet = new HashSet<String>(existing, StringComparer.OrdinalIgnoreCase);

        var toCreate = normalized
        .Where(x => !existingSet.Contains(x))
        .ToList();

        if (toCreate.Count == 0)
            return new List<RadioOptionDto>();

        var entities = toCreate
            .Select(name => new RadioOption(GuidGenerator.Create(), input.ListItemId, name))
            .ToList();

        await _repository.InsertManyAsync(entities, autoSave: true);

        return ObjectMapper.Map<List<RadioOption>, List<RadioOptionDto>>(entities);
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
