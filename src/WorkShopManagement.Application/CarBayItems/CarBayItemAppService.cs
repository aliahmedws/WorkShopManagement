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
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.CarBayItems;

[RemoteService(IsEnabled = false)]
[Authorize(WorkShopManagementPermissions.CarBayItems.Default)]
public class CarBayItemAppService : ApplicationService, ICarBayItemAppService
{
    private readonly IRepository<CarBayItem, Guid> _carBayItemRepository;
    private readonly IEntityAttachmentService _entityAttachmentAppService;

    public CarBayItemAppService(
        IRepository<CarBayItem, Guid> carBayItemRepository,
        IEntityAttachmentService entityAttachmentAppService)
    {
        _carBayItemRepository = carBayItemRepository;
        _entityAttachmentAppService = entityAttachmentAppService;
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

        foreach (var dto in dtos)
        {
            var attachments = await _entityAttachmentAppService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = dto.Id,
                EntityType = EntityType.CarBayItem
            });

            dto.EntityAttachments = attachments!;
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
}
