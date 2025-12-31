using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.EntityAttachments;

[RemoteService(IsEnabled = false)]
public class EntityAttachmentAppService : ApplicationService, IEntityAttachmentAppService
{
    private readonly IRepository<EntityAttachment, Guid> _repository;
    private readonly FileManager _fileManager;

    public EntityAttachmentAppService(
        IRepository<EntityAttachment, Guid> repository,
        FileManager fileManager)
    {
        _repository = repository;
        _fileManager = fileManager;
    }

    public async Task<List<EntityAttachmentDto>> GetListAsync(GetEntityAttachmentListDto input)
    {

        var queryable = await _repository.GetQueryableAsync();

        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.EntityId == input.EntityId && x.EntityType.Equals(input.EntityType))
        );

        return ObjectMapper.Map<List<EntityAttachment>, List<EntityAttachmentDto>>(items);
    }


    public async Task<List<EntityAttachmentDto>> UploadAttachmentsAsync(UploadAttachmentDto input, List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
        {
            throw new UserFriendlyException("No file(s) provided.");
        }

        var entities = new List<EntityAttachment>(files.Count);

        foreach (var f in files)
        {
            using var stream = new MemoryStream();
            await f.CopyToAsync(stream);
            stream.Position = 0;

            var saved = await _fileManager.SaveAsync(stream, f.FileName, "listitem-attachments");

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

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);

        if (entity.Attachment != null && !string.IsNullOrWhiteSpace(entity.Attachment.Path))
        {
            await _fileManager.DeleteFileAsync(entity.Attachment);
        }

        await _repository.DeleteAsync(entity);
    }

}
