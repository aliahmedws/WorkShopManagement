using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using WorkShopManagement.ListItems;

namespace WorkShopManagement.FileAttachments;

public class FileAttachmentManager : DomainService
{
    private readonly IRepository<ListItem, Guid> _repository;
    private readonly FileManager _fileManager;

    public FileAttachmentManager(IRepository<ListItem, Guid> repository, FileManager fileManager)
    {
        _repository = repository;
        _fileManager = fileManager;
    }

    public async Task<ListItem> SetAttachmentAsync(Guid entityId, IFormFile file)
    {
        Check.NotNull(file, nameof(file));

        var entity = await _repository.GetAsync(entityId);

        if (entity.Attachment != null && !string.IsNullOrWhiteSpace(entity.Attachment.Path))
        {
            await _fileManager.DeleteFileAsync(entity.Attachment);
            entity.ClearAttachment();
        }

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        var attachment = await _fileManager.SaveAsync(stream, file.FileName, "attachments");

        entity.SetAttachment(attachment);

        await _repository.UpdateAsync(entity, autoSave: true);
        return entity;
    }

    public async Task RemoveAttachmentAsync(Guid entityId)
    {
        var entity = await _repository.GetAsync(entityId);

        if (entity.Attachment != null && !string.IsNullOrWhiteSpace(entity.Attachment.Path))
        {
            await _fileManager.DeleteFileAsync(entity.Attachment);
            entity.ClearAttachment();
            await _repository.UpdateAsync(entity, autoSave: true);
        }
    }
}
