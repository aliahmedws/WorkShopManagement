using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.Permissions;
using WorkShopManagement.RadioOptions;

namespace WorkShopManagement.ListItems;

[RemoteService(isEnabled: false)]

[Authorize(WorkShopManagementPermissions.ListItems.Default)]
public class ListItemAppService : ApplicationService, IListItemAppService
{
    private readonly IRepository<ListItem, Guid> _repository;
    private readonly IEntityAttachmentService _entityAttachmentAppService;

    public ListItemAppService(IRepository<ListItem, Guid> repository, IEntityAttachmentService entityAttachmentAppService)
    {
        _repository = repository;
        _entityAttachmentAppService = entityAttachmentAppService;
    }

    public async Task<PagedResultDto<ListItemDto>> GetListAsync(GetListItemListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();

        if(!input.Filter.IsNullOrWhiteSpace())
        {
            var f = input.Filter.Trim().ToLower();
            queryable = queryable.Where(x => x.Name.ToLower().Contains(input.Filter));
        }

        if (input.CheckListId.HasValue)
        {
            queryable = queryable.Where(x => x.CheckListId == input.CheckListId.Value);
        }

        if (!input.Filter.IsNullOrWhiteSpace())
        {
            var filter = input.Filter.Trim();
            queryable = queryable.Where(x => x.Name.Contains(filter) || x.CommentPlaceholder!.Contains(filter));
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

        foreach(var dto in dtos)
        {
            var attachments = await _entityAttachmentAppService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = dto.Id,
                EntityType = EntityType.ListItem
            });

            dto.EntityAttachments = attachments!;
        }
        return new PagedResultDto<ListItemDto>(totalCount, dtos);
    }

    public async Task<ListItemDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        var dto = ObjectMapper.Map<ListItem, ListItemDto>(entity);

        var attachments = await _entityAttachmentAppService.GetListAsync(new GetEntityAttachmentListDto
        {
            EntityId = id,
            EntityType = EntityType.ListItem
        });

        dto.EntityAttachments = attachments!;
        return dto;
    }


    [Authorize(WorkShopManagementPermissions.ListItems.Create)]
    public async Task<ListItemDto> CreateAsync(CreateListItemDto input)
    {
        var name = input.Name?.Trim();
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("Name is required.");
        }

        var placeHolder = input.CommentPlaceholder.IsNullOrWhiteSpace()
            ? name
            : input.CommentPlaceholder.Trim();

        if (input.IsSeparator == true)
        {
            input.IsAttachmentRequired = false;

            placeHolder = null;
        }

        var normalizedName = name.ToUpperInvariant();

        var exists = await _repository.AnyAsync(x =>
            x.CheckListId == input.CheckListId &&
            x.Name.ToUpper() == normalizedName);

        if (exists)
        {
            throw new UserFriendlyException($"List item '{name}' already exists in this checklist.");
        }

        var positionExists = await _repository.AnyAsync(x =>
            x.CheckListId == input.CheckListId &&
            x.Position == input.Position);

        if (positionExists)
        {
            throw new UserFriendlyException($"Position '{input.Position}' already exists for this checklist.");
        }

        var entity = new ListItem(
            GuidGenerator.Create(),
            input.CheckListId,
            input.Position,
            name,
            placeHolder,
            input.CommentType,
            input.IsAttachmentRequired,
            input.IsSeparator
        );

        entity = await _repository.InsertAsync(entity, autoSave: true);

        // --- CREATE EntityAttachment 
        await _entityAttachmentAppService.CreateAsync(null, new CreateAttachmentDto
        {
            EntityType = EntityType.ListItem,
            EntityId = entity.Id,
            TempFiles = input.TempFiles
        });
        // --- create end

        //return await GetAsync(entity.Id);
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

        if (input.IsSeparator == true)
        {
            input.IsAttachmentRequired = false;

            placeholder = null;
        }

        var normalizedName = name.ToUpperInvariant();

        var exists = await _repository.AnyAsync(x =>
            x.Id != id &&
            x.CheckListId == entity.CheckListId && 
            x.Name.ToUpper() == normalizedName);

        if (exists)
        {
            throw new UserFriendlyException($"List item '{name}' already exists in this checklist.");
        }

        var positionExists = await _repository.AnyAsync(x =>
           x.Id != id &&
           x.CheckListId == input.CheckListId &&
           x.Position == input.Position);

        if (positionExists)
        {
            throw new UserFriendlyException($"Position '{input.Position}' already exists for this checklist.");
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
            input.IsAttachmentRequired,
            input.IsSeparator
        );

        await _repository.UpdateAsync(entity, autoSave: true);

        // --- UPDATE EntityAttachment 
        await _entityAttachmentAppService.UpdateAsync(null, new UpdateEntityAttachmentDto
        {
            EntityId = entity.Id,
            EntityType = EntityType.ListItem,
            TempFiles = input.TempFiles,
            EntityAttachments = input.EntityAttachments
        });
        // --- update end

        return ObjectMapper.Map<ListItem, ListItemDto>(entity);
    }


    [Authorize(WorkShopManagementPermissions.ListItems.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _entityAttachmentAppService.DeleteAsync(id, EntityType.ListItem, null);
        await _repository.DeleteAsync(id);
    }

    public async Task<List<ListItemDto>> GetByCheckListWithDetailsAsync(Guid checkListId)
    {
        var queryable = await _repository.WithDetailsAsync();

        var items = await AsyncExecuter.ToListAsync(
            queryable.Include(x => x.RadioOptions)
                .Where(x => x.CheckListId == checkListId)
                .OrderBy(x => x.Position)
        );

        var dtos = ObjectMapper.Map<List<ListItem>, List<ListItemDto>>(items);

        foreach (var dto in dtos)
        {
            var attachments = await _entityAttachmentAppService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = dto.Id,
                EntityType = EntityType.ListItem
            });

            dto.EntityAttachments = attachments!;

            dto.RadioOptions = dto.RadioOptions?
                .OrderBy(x =>
                {
                    var name = x.Name.ToUpperInvariant();
                    if (name == "N/A")
                        return int.MaxValue;
                    if (name.StartsWith("OTHER"))
                        return int.MaxValue - 1;
                    var i = Array.IndexOf(RadioOptionConsts.OrderedNames, name);
                    return i < 0 ? int.MaxValue - 2 : i;
                })
                .ThenBy(x => x.Name)
                .ToList();
        }

        return dtos;
    }


}
