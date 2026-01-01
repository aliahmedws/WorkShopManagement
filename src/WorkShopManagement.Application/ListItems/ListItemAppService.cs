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

namespace WorkShopManagement.ListItems;

[RemoteService(isEnabled: false)]

[Authorize(WorkShopManagementPermissions.ListItems.Default)]
public class ListItemAppService : ApplicationService, IListItemAppService
{
    private readonly IRepository<ListItem, Guid> _repository;

    public ListItemAppService(IRepository<ListItem, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<ListItemDto>> GetListAsync(GetListItemListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();

        if (input.CheckListId.HasValue)
        {
            queryable = queryable.Where(x => x.CheckListId == input.CheckListId.Value);
        }

        if (!input.Filter.IsNullOrWhiteSpace())
        {
            var filter = input.Filter.Trim();
            queryable = queryable.Where(x => x.Name.Contains(filter) || x.CommentPlaceholder.Contains(filter));
        }

        var sorting = input.Sorting.IsNullOrWhiteSpace()
            ? $"{nameof(ListItem.CheckListId)}, {nameof(ListItem.Position)}"
            : input.Sorting;

        queryable = queryable.OrderBy(sorting);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var items = await AsyncExecuter.ToListAsync(
            queryable
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        var dtos = ObjectMapper.Map<List<ListItem>, List<ListItemDto>>(items);
        return new PagedResultDto<ListItemDto>(totalCount, dtos);
    }

    public async Task<ListItemDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<ListItem, ListItemDto>(entity);
    }


    [Authorize(WorkShopManagementPermissions.ListItems.Create)]
    public async Task<ListItemDto> CreateAsync(CreateListItemDto input)
    {
        var name = input.Name?.Trim();
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("Name is required.");
        }

        var placeholder = input.CommentPlaceholder.IsNullOrWhiteSpace()
            ? name
            : input.CommentPlaceholder!.Trim();

        if (input.IsSeparator)
        {
            input.IsAttachmentRequired = false;
        }

        var normalizedName = name.ToUpperInvariant();

        var exists = await _repository.AnyAsync(x =>
            x.CheckListId == input.CheckListId &&
            x.Name.ToUpper() == normalizedName);

        if (exists)
        {
            throw new UserFriendlyException($"List item '{name}' already exists in this checklist.");
        }

        var entity = new ListItem(
            GuidGenerator.Create(),
            input.CheckListId,
            input.Position,
            name,
            placeholder,
            input.CommentType,
            input.IsAttachmentRequired,
            input.IsSeparator
        );

        await _repository.InsertAsync(entity, autoSave: true);

        return ObjectMapper.Map<ListItem, ListItemDto>(entity);
    }


    [Authorize(WorkShopManagementPermissions.ListItems.Edit)]
    public async Task<ListItemDto> UpdateAsync(Guid id, UpdateListItemDto input)
    {
        var entity = await _repository.GetAsync(id);

        var name = input.Name?.Trim();
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("Name is required.");
        }

        var placeholder = input.CommentPlaceholder.IsNullOrWhiteSpace()
            ? name
            : input.CommentPlaceholder!.Trim();

        var isAttachmentRequired = input.IsSeparator ? false : input.IsAttachmentRequired;

        var normalizedName = name.ToUpperInvariant();

        var exists = await _repository.AnyAsync(x =>
            x.Id != id &&
            x.CheckListId == entity.CheckListId && 
            x.Name.ToUpper() == normalizedName);

        if (exists)
        {
            throw new UserFriendlyException($"List item '{name}' already exists in this checklist.");
        }

        if (!input.ConcurrencyStamp.IsNullOrWhiteSpace())
        {
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
        }

        entity.Update(
            input.Position,
            name,
            placeholder,
            input.CommentType,
            isAttachmentRequired,
            input.IsSeparator
        );

        await _repository.UpdateAsync(entity, autoSave: true);

        return ObjectMapper.Map<ListItem, ListItemDto>(entity);
    }


    [Authorize(WorkShopManagementPermissions.ListItems.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
    
}
