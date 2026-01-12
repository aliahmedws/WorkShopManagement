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
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using WorkShopManagement.CarBayItems.BatchCarBayItems;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.CarBayItems;

[RemoteService(IsEnabled = false)]
[Authorize(WorkShopManagementPermissions.CarBayItems.Default)]
public class CarBayItemAppService : ApplicationService, ICarBayItemAppService
{
    private readonly IRepository<CarBayItem, Guid> _carBayItemRepository;
    private readonly IEntityAttachmentService _entityAttachmentAppService;
    private readonly IIdentityUserRepository _identityUserRepository;

    public CarBayItemAppService(
        IRepository<CarBayItem, Guid> carBayItemRepository,
        IEntityAttachmentService entityAttachmentAppService,
         IIdentityUserRepository identityUserRepository)
    {
        _carBayItemRepository = carBayItemRepository;
        _entityAttachmentAppService = entityAttachmentAppService;
        _identityUserRepository = identityUserRepository;
    }

    public async Task<CarBayItemDto> GetAsync(Guid id)
    {
        var queryable = await _carBayItemRepository.WithDetailsAsync(x => x.ListItem!, x => x.CarBay!);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == id));

        if (entity == null)
            throw new EntityNotFoundException(typeof(CarBayItem), id);

        var dto = ObjectMapper.Map<CarBayItem, CarBayItemDto>(entity);

        var attachments = await _entityAttachmentAppService.GetListAsync(new GetEntityAttachmentListDto
        {
            EntityId = id,
            EntityType = EntityType.CarBayItem
        });

        dto.EntityAttachments = attachments!;
        return dto;
    }

    public async Task<PagedResultDto<CarBayItemDto>> GetListAsync(GetCarBayItemListDto input)
    {
        var queryable = await _carBayItemRepository.WithDetailsAsync(x => x.ListItem!, x => x.CarBay!);

        if (input.CarBayId.HasValue)
            queryable = queryable.Where(x => x.CarBayId == input.CarBayId.Value);

        if (input.CheckListItemId.HasValue)
            queryable = queryable.Where(x => x.CheckListItemId == input.CheckListItemId.Value);

        if (!input.Filter.IsNullOrWhiteSpace())
        {
            var f = input.Filter.Trim();

            queryable = queryable.Where(x =>
                x.Id.ToString() == f ||
                x.CarBayId.ToString() == f ||
                x.CheckListItemId.ToString() == f ||
                (x.CheckRadioOption != null && x.CheckRadioOption.Contains(f)) ||
                (x.Comments != null && x.Comments.Contains(f)) ||
                (x.ListItem != null && x.ListItem.Name != null && x.ListItem.Name.Contains(f))
            );
        }

        var sorting = input.Sorting.IsNullOrWhiteSpace()
            ? $"{nameof(CarBayItem.CreationTime)} desc"
            : input.Sorting;

        queryable = queryable.OrderBy(sorting);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var items = await AsyncExecuter.ToListAsync(
            queryable
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        var dtos = ObjectMapper.Map<List<CarBayItem>, List<CarBayItemDto>>(items);

        IdentityUser modifier = null;
        IdentityUser creator = null;

        foreach (var item in items)
        {
            if (item.LastModifierId.HasValue)
            {
                modifier = await _identityUserRepository.FindAsync(item.LastModifierId.Value);
            }

            if (item.CreatorId.HasValue)
            {
                creator = await _identityUserRepository.FindAsync(item.CreatorId.Value);
            }

        }

        foreach (var dto in dtos)
        {
            var attachments = await _entityAttachmentAppService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = dto.Id,
                EntityType = EntityType.CarBayItem
            });

            dto.EntityAttachments = attachments!;

            if(modifier != null && modifier.UserName != null)
            {
                dto.ModifierName = modifier!.UserName;
            }

            if(creator != null && creator.UserName != null)
            {
                dto.CreatorName = creator!.UserName;
            }

        }

        return new PagedResultDto<CarBayItemDto>(totalCount, dtos);
    }

    [Authorize(WorkShopManagementPermissions.CarBayItems.Create)]
    public async Task<CarBayItemDto> CreateAsync(CreateCarBayItemDto input)
    {
        var exists = await _carBayItemRepository.AnyAsync(x =>
            x.CarBayId == input.CarBayId &&
            x.CheckListItemId == input.CheckListItemId
        );

        if (exists)
            throw new UserFriendlyException("This checklist item already exists for this CarBay.");

        var entity = new CarBayItem(
            GuidGenerator.Create(),
            input.CheckListItemId,
            input.CarBayId,
            input.CheckRadioOption?.Trim(),
            input.Comments?.Trim()
        );

        entity = await _carBayItemRepository.InsertAsync(entity, autoSave: true);

        await _entityAttachmentAppService.CreateAsync(new CreateAttachmentDto
        {
            EntityType = EntityType.CarBayItem,
            EntityId = entity.Id,
            TempFiles = input.TempFiles
        });

        var dto = ObjectMapper.Map<CarBayItem, CarBayItemDto>(entity);
        dto.EntityAttachments = await _entityAttachmentAppService.GetListAsync(new GetEntityAttachmentListDto
        {
            EntityId = entity.Id,
            EntityType = EntityType.CarBayItem
        }) ?? new List<EntityAttachmentDto>();

        return dto;
    }

    [Authorize(WorkShopManagementPermissions.CarBayItems.Edit)]
    public async Task<CarBayItemDto> UpdateAsync(Guid id, UpdateCarBayItemDto input)
    {
        var entity = await _carBayItemRepository.GetAsync(id);

        var exists = await _carBayItemRepository.AnyAsync(x =>
         x.CarBayId == input.CarBayId &&
         x.CheckListItemId == input.CheckListItemId &&
         x.Id != entity.Id
        );

        if (exists)
            throw new UserFriendlyException("This checklist item already exists for this CarBay.");

        entity.SetCarBay(input.CarBayId);
        entity.SetCheckListItem(input.CheckListItemId);
        entity.SetCheckRadioOption(input.CheckRadioOption);
        entity.SetComments(input.Comments);
        

        if (!input.ConcurrencyStamp.IsNullOrWhiteSpace())
        {
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
        }

        await _carBayItemRepository.UpdateAsync(entity, autoSave: true);

        await _entityAttachmentAppService.UpdateAsync(new UpdateEntityAttachmentDto
        {
            EntityId = entity.Id,
            EntityType = EntityType.CarBayItem,
            TempFiles = input.TempFiles,
            EntityAttachments = input.EntityAttachments
        });

        var dto = ObjectMapper.Map<CarBayItem, CarBayItemDto>(entity);
        dto.EntityAttachments = await _entityAttachmentAppService.GetListAsync(new GetEntityAttachmentListDto
        {
            EntityId = entity.Id,
            EntityType = EntityType.CarBayItem
        }) ?? new List<EntityAttachmentDto>();

        return dto;
    }

    [Authorize(WorkShopManagementPermissions.CarBayItems.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _entityAttachmentAppService.DeleteAsync(id, EntityType.CarBayItem);
        await _carBayItemRepository.DeleteAsync(id);
    }

    [Authorize(WorkShopManagementPermissions.CarBayItems.Edit)]
    public async Task<SaveCarBayItemBatchResultDto> SaveBatchAsync(SaveCarBayItemBatchDto input)
    {
        if (input.Items == null || input.Items.Count == 0)
            return new SaveCarBayItemBatchResultDto();

        // Basic validation: no duplicates in the same payload
        var dup = input.Items
            .GroupBy(x => new { x.CarBayId, x.CheckListItemId })
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .FirstOrDefault();

        if (dup != null)
            throw new UserFriendlyException("Duplicate checklist items found in the request for the same CarBay.");

        // For performance: load existing items for this CarBay in one query
        var carBayId = input.Items[0].CarBayId;
        var query = await _carBayItemRepository.GetQueryableAsync();

        var existing = await AsyncExecuter.ToListAsync(
            query.Where(x => x.CarBayId == carBayId)
        );

        // Build lookup by (CarBayId, CheckListItemId)
        var existingByKey = existing.ToDictionary(
            x => (x.CarBayId, x.CheckListItemId),
            x => x
        );

        var resultDtos = new List<CarBayItemDto>();

        // Single UoW transaction for all rows
        // (ABP ApplicationService already has UoW by default, but explicit is OK when doing batch ops)
        foreach (var row in input.Items)
        {
            // ensure payload consistent
            if (row.CarBayId == Guid.Empty)
                row.CarBayId = carBayId;

            CarBayItem entity;

            // Prefer ID match if provided
            if (row.Id.HasValue)
            {
                entity = existing.FirstOrDefault(x => x.Id == row.Id.Value)
                         ?? await _carBayItemRepository.GetAsync(row.Id.Value);

                // update fields
                entity.SetCarBay(row.CarBayId);
                entity.SetCheckListItem(row.CheckListItemId);
                entity.SetCheckRadioOption(row.CheckRadioOption?.Trim());
                entity.SetComments(row.Comments?.Trim());

                if (!row.ConcurrencyStamp.IsNullOrWhiteSpace())
                    entity.SetConcurrencyStampIfNotNull(row.ConcurrencyStamp);

                await _carBayItemRepository.UpdateAsync(entity, autoSave: true);
            }
            else
            {
                // Create-or-update by business key (CarBayId + CheckListItemId)
                if (existingByKey.TryGetValue((row.CarBayId, row.CheckListItemId), out var found))
                {
                    entity = found;

                    entity.SetCheckRadioOption(row.CheckRadioOption?.Trim());
                    entity.SetComments(row.Comments?.Trim());

                    if (!row.ConcurrencyStamp.IsNullOrWhiteSpace())
                        entity.SetConcurrencyStampIfNotNull(row.ConcurrencyStamp);

                    await _carBayItemRepository.UpdateAsync(entity, autoSave: true);
                }
                else
                {
                    entity = new CarBayItem(
                        GuidGenerator.Create(),
                        row.CheckListItemId,
                        row.CarBayId,
                        row.CheckRadioOption?.Trim(),
                        row.Comments?.Trim()
                    );

                    entity = await _carBayItemRepository.InsertAsync(entity, autoSave: true);
                    existingByKey[(entity.CarBayId, entity.CheckListItemId)] = entity;
                    existing.Add(entity);
                }
            }

            // Attachments: upsert per row
            // If your attachment service needs "Create" for new and "Update" for modify,
            // we can just call Update always (if your service supports it), otherwise keep as below.

            // 1) Add new temp files (if any)
            if (row.TempFiles != null && row.TempFiles.Count > 0)
            {
                await _entityAttachmentAppService.CreateAsync(new CreateAttachmentDto
                {
                    EntityType = EntityType.CarBayItem,
                    EntityId = entity.Id,
                    TempFiles = row.TempFiles
                });
            }

            // 2) Apply final attachment set (keep/delete) if UI sends existing list
            if (row.EntityAttachments.Count > 0)
            {
                await _entityAttachmentAppService.UpdateAsync(new UpdateEntityAttachmentDto
                {
                    EntityId = entity.Id,
                    EntityType = EntityType.CarBayItem,
                    TempFiles = null, // already handled above
                    EntityAttachments = row.EntityAttachments
                });
            }

            // Map result (optionally include attachments)
            var dto = ObjectMapper.Map<CarBayItem, CarBayItemDto>(entity);

            dto.EntityAttachments = await _entityAttachmentAppService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = entity.Id,
                EntityType = EntityType.CarBayItem
            }) ?? new List<EntityAttachmentDto>();

            resultDtos.Add(dto);
        }

        return new SaveCarBayItemBatchResultDto
        {
            Items = resultDtos
        };
    }
}
