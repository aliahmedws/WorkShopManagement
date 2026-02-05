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

    [Authorize(WorkShopManagementPermissions.RadioOptions.Edit)] // or Create+Edit as you prefer
    public async Task<List<RadioOptionDto>> UpsertAsync(UpsertRadioOptionsDto input)
    {
        if (input.Items == null || input.Items.Count == 0)
            return new List<RadioOptionDto>();

        // 1) Normalize and dedupe incoming by Name (case-insensitive)
        var normalized = input.Items
            .Select(x => new
            {
                Name = x.Name?.Trim(),
                x.IsAcceptable
            })
            .Where(x => !x.Name.IsNullOrWhiteSpace())
            .GroupBy(x => x.Name!, StringComparer.OrdinalIgnoreCase)
            // if duplicates in payload, last one wins (or change logic as needed)
            .Select(g => g.Last())
            .ToList();

        if (normalized.Count == 0)
            return new List<RadioOptionDto>();

        var incomingNames = normalized.Select(x => x.Name!).ToList();

        // 2) Load existing options for these names under this ListItem
        var q = await _repository.GetQueryableAsync();

        var existing = await AsyncExecuter.ToListAsync(q.Where(x => x.ListItemId == input.ListItemId && incomingNames.Contains(x.Name)));

        // NOTE: incomingNames.Contains(x.Name) is case-sensitive in SQL for some collations.
        // We'll match in-memory case-insensitively to be safe.
        var existingByName = existing
            .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var toInsert = new List<RadioOption>();
        var toUpdate = new List<RadioOption>();

        foreach (var item in normalized)
        {
            if (existingByName.TryGetValue(item.Name!, out var entity))
            {
                // Update existing: keep invariant logic for Name if you want to normalize casing
                // entity.ChangeName(item.Name!); // optional: only if you want to update stored casing
                entity.SetIsAcceptable(item.IsAcceptable);

                toUpdate.Add(entity);
            }
            else
            {
                toInsert.Add(new RadioOption(
                    GuidGenerator.Create(),
                    input.ListItemId,
                    item.Name!,
                    item.IsAcceptable
                ));
            }
        }

        // 3) Persist
        if (toInsert.Count > 0)
            await _repository.InsertManyAsync(toInsert, autoSave: false);

        // Updates are tracked in EF Core repos, but UpdateMany is fine and explicit.
        if (toUpdate.Count > 0)
            await _repository.UpdateManyAsync(toUpdate, autoSave: false);

        var result = toUpdate.Concat(toInsert).ToList();
        return ObjectMapper.Map<List<RadioOption>, List<RadioOptionDto>>(result);
    }

    //[Authorize(WorkShopManagementPermissions.RadioOptions.Create)]
    //public async Task<List<RadioOptionDto>> CreateAsync(CreateRadioOptionDto input)
    //{
    //    if (input.Names == null || input.Names.Count == 0)
    //        return new List<RadioOptionDto>();

    //    var normalized = input.Names
    //        .Select(x => x?.Trim())
    //        .Where(x => !x.IsNullOrWhiteSpace())
    //        .Distinct(StringComparer.OrdinalIgnoreCase)
    //        .ToList();

    //    if (normalized.Count == 0)
    //        return new List<RadioOptionDto>();

    //    var queryable = await _repository.GetQueryableAsync();
    //    var existing = await AsyncExecuter.ToListAsync(
    //        queryable.Where(x => x.ListItemId == input.ListItemId)
    //        .Select(x => x.Name));

    //    var existingSet = new HashSet<String>(existing, StringComparer.OrdinalIgnoreCase);

    //    var toCreate = normalized
    //    .Where(x => !existingSet.Contains(x))
    //    .ToList();

    //    if (toCreate.Count == 0)
    //        return new List<RadioOptionDto>();

    //    var entities = toCreate
    //        .Select(name => new RadioOption(GuidGenerator.Create(), input.ListItemId, name))
    //        .ToList();

    //    await _repository.InsertManyAsync(entities, autoSave: true);

    //    return ObjectMapper.Map<List<RadioOption>, List<RadioOptionDto>>(entities);
    //}

    //[Authorize(WorkShopManagementPermissions.RadioOptions.Edit)]
    //public async Task<RadioOptionDto> UpdateAsync(Guid id, UpdateRadioOptionDto input)
    //{
    //    var entity = await _repository.GetAsync(id);
    //    var name = input.Name?.Trim();

    //    var exists = await _repository.AnyAsync(x =>
    //        x.Id != id &&
    //        x.ListItemId == entity.ListItemId &&
    //        x.Name == name);

    //    if (exists)
    //    {
    //        throw new UserFriendlyException($"Radio option '{name}' already exists for this list item.");
    //    }

    //    if (entity.ConcurrencyStamp.IsNullOrWhiteSpace())
    //    {
    //        entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
    //    }

    //    entity.ChangeName(name!);
    //    await _repository.UpdateAsync(entity, autoSave: true);

    //    return ObjectMapper.Map<RadioOption, RadioOptionDto>(entity);
    //}

    [Authorize(WorkShopManagementPermissions.RadioOptions.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}
