using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace WorkShopManagement.FileAttachments;

public class FileAttachmentManager : DomainService
{
    private readonly IRepository<EntityAttachment, Guid> _repository;
    private readonly FileManager _fileManager;

    public FileAttachmentManager(IRepository<EntityAttachment, Guid> repository, FileManager fileManager)
    {
        _repository = repository;
        _fileManager = fileManager;
    }

    public async Task<EntityAttachment> UploadForListItemAsync(Guid listItemId, IFormFile file)
    {
        Check.NotNull(file, nameof(file));

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        var saved = await _fileManager.SaveAsync(stream, file.FileName, "listitem-attachments");

        var entity = new EntityAttachment(
            id: GuidGenerator.Create(),
            checkListId: null,
            listItemId: listItemId,
            attachment: saved
        );

        await _repository.InsertAsync(entity, autoSave: true);
        return entity;
    }

    public async Task<EntityAttachment> UploadForCheckListAsync(Guid checkListId, IFormFile file)
    {
        Check.NotNull(file, nameof(file));

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        var saved = await _fileManager.SaveAsync(stream, file.FileName, "checklist-attachments");

        var entity = new EntityAttachment(
            id: GuidGenerator.Create(),
            checkListId: checkListId,
            listItemId: null,
            attachment: saved
        );

        await _repository.InsertAsync(entity, autoSave: true);
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);

        if (!string.IsNullOrWhiteSpace(entity.Attachment?.Path))
        {
            await _fileManager.DeleteFileAsync(entity.Attachment);
        }

        await _repository.DeleteAsync(entity, autoSave: true);
    }
}
