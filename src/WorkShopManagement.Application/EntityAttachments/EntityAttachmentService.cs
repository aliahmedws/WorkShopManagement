using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments.Files;

namespace WorkShopManagement.EntityAttachments;

[RemoteService(IsEnabled = false)]
public class EntityAttachmentService(
    IRepository<EntityAttachment, Guid> repository,
    FileManager fileManager) : ApplicationService, IEntityAttachmentService
{
    private readonly IRepository<EntityAttachment, Guid> _repository = repository;
    private readonly FileManager _fileManager = fileManager;

    public async Task<List<EntityAttachmentDto>> GetListAsync(GetEntityAttachmentListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();

        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.EntityId == input.EntityId && x.EntityType.Equals(input.EntityType))
        );

        return ObjectMapper.Map<List<EntityAttachment>, List<EntityAttachmentDto>>(items);
    }
    public async Task<List<EntityAttachmentDto>> GetListAsync(Guid entityId, string entityType)
    {

        var queryable = await _repository.GetQueryableAsync();

        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.EntityId == entityId && x.EntityType.Equals(entityType))
        );

        return ObjectMapper.Map<List<EntityAttachment>, List<EntityAttachmentDto>>(items);
    }
    public async Task DeleteAsync(Guid entityId, EntityType entityType)
    {
        var queryable = await _repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.EntityId == entityId && x.EntityType.Equals(entityType))
        );

        if (items != null && items.Count != 0)
        {
            foreach (var item in items)
            {
                await _fileManager.DeleteAsync(item.Attachment);
            }

            var ids = items.Select(x => x.Id).ToList();
            await _repository.DeleteManyAsync(ids);
        }
    }
    public async Task<List<EntityAttachmentDto>> CreateAsync(CreateAttachmentDto input)
    {
        if (input.TempFiles == null || input.TempFiles.Count == 0)
        {
            return [];
        }

        var entities = new List<EntityAttachment>(input.TempFiles.Count);

        foreach (var f in input.TempFiles)
        {
            var saved = await _fileManager.SaveFromTempAsync(f.Name, f.BlobName);

            entities.Add(new EntityAttachment(
                id: GuidGenerator.Create(),
                entityId: input.EntityId,
                entityType: input.EntityType,
                attachment: saved
            ));
        }

        await _repository.InsertManyAsync(entities, autoSave: true);

        return ObjectMapper.Map<List<EntityAttachment>, List<EntityAttachmentDto>>(entities);
    }



    public async Task<List<EntityAttachmentDto>> UpdateAsync(UpdateEntityAttachmentDto input)
    {

        var keptIds = input.Attachments?.Select(x => x.Id).ToHashSet() ?? new HashSet<Guid>();

        var queryable = await _repository.GetQueryableAsync();
        var dbItems = await AsyncExecuter.ToListAsync(
            queryable
                //.AsNoTracking()
                .Where(x => x.EntityType.Equals(input.EntityType) && x.EntityId == input.EntityId && !keptIds.Contains(x.Id))
                .Select(x => new { x.Id, Name = x.Attachment.Name, BlobName = x.Attachment.BlobName, Path = x.Attachment.Path  })
        );

        if (dbItems != null && dbItems.Count != 0)
        {
            foreach (var dbItem in dbItems)
            {
                await _fileManager.DeleteAsync(new FileAttachment(dbItem.Name, dbItem.BlobName, dbItem.Path));

            }
            var ids = dbItems.Select(x => x.Id).ToList();
            await _repository.DeleteManyAsync(ids);
        }


        var newItems = await CreateAsync(new CreateAttachmentDto
        {
            EntityId = input.EntityId,
            EntityType = input.EntityType,
            TempFiles = input.TempFiles
        });

        return await GetListAsync(new GetEntityAttachmentListDto
        {
            EntityId = input.EntityId,
            EntityType = input.EntityType
        });
    }

}
