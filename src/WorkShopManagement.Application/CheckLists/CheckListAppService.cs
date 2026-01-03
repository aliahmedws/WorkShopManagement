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
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.CheckLists;

[RemoteService(IsEnabled = false)]
[Authorize(WorkShopManagementPermissions.CheckLists.Default)]
public class CheckListAppService : ApplicationService, ICheckListAppService
{
    private readonly IRepository<CheckList, Guid> _checkListRepository;
    private readonly IEntityAttachmentService _entityAttachmentAppService;

    public CheckListAppService(IRepository<CheckList, Guid> checkListRepository
        , IEntityAttachmentService entityAttachmentAppService
        )
    {
        _checkListRepository = checkListRepository;
        _entityAttachmentAppService = entityAttachmentAppService;
    }

    public async Task<PagedResultDto<CheckListDto?>> GetListAsync(GetCheckListListDto input)
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
        foreach (var dto in dtos)
        {
            var attachments = await _entityAttachmentAppService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = dto.Id,
                EntityType = EntityType.CheckList
            });
            dto.EntityAttachments = attachments!;
        }
        return new PagedResultDto<CheckListDto?>(totalCount, dtos);
    }

    public async Task<CheckListDto> GetAsync(Guid id)
    {
        var entity = await _checkListRepository.GetAsync(id);
        var dtos = ObjectMapper.Map<CheckList, CheckListDto>(entity);

        var attachments = await _entityAttachmentAppService.GetListAsync(new GetEntityAttachmentListDto
        {
            EntityId = id,
            EntityType = EntityType.CheckList
        });

        dtos.EntityAttachments = attachments!;

        return dtos;
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
            input.CarModelId
        );

        entity = await _checkListRepository.InsertAsync(entity, autoSave: true);

        // --- CREATE EntityAttachment 
        await _entityAttachmentAppService.CreateAsync(new CreateAttachmentDto
        {
            EntityType = EntityType.CheckList,
            EntityId = entity.Id,
            TempFiles = input.TempFiles
        });
        // --- create end

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

        entity.CarModelId = input.CarModelId;
        entity.ChangeName(name!);
        entity.ChangePosition(input.Position);

        if (!input.ConcurrencyStamp.IsNullOrWhiteSpace())
        {
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
        }

        await _checkListRepository.UpdateAsync(entity, autoSave: true);

        // --- UPDATE EntityAttachment 
        await _entityAttachmentAppService.UpdateAsync(new UpdateEntityAttachmentDto
        {
            EntityId = entity.Id,
            EntityType = EntityType.CheckList,
            TempFiles = input.TempFiles,
            EntityAttachments = input.Attachments
        });
        // --- update end
        return ObjectMapper.Map<CheckList, CheckListDto>(entity);
    }

    [Authorize(WorkShopManagementPermissions.CheckLists.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _entityAttachmentAppService.DeleteAsync(id, EntityType.CheckList);
        await _checkListRepository.DeleteAsync(id);
    }
}
