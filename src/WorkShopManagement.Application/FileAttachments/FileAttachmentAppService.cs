using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.FileAttachments;

[RemoteService(IsEnabled = false)]
public class FileAttachmentAppService : ApplicationService, IEntityAttachmentAppService
{
    private readonly IRepository<EntityAttachment, Guid> _repository;
    private readonly FileManager _fileManager;

    public FileAttachmentAppService(
        IRepository<EntityAttachment, Guid> repository,
        FileManager fileManager)
    {
        _repository = repository;
        _fileManager = fileManager;
    }

    public async Task<PagedResultDto<EntityAttachmentDto>> GetListAsync(GetEntityAttachmentListDto input)
    {
        if (!input.CheckListId.HasValue && !input.ListItemId.HasValue)
        {
            throw new UserFriendlyException("Either CheckListId or ListItemId is required.");
        }

        if (input.CheckListId.HasValue && input.ListItemId.HasValue)
        {
            throw new UserFriendlyException("Provide only one: CheckListId or ListItemId.");
        }

        var queryable = await _repository.GetQueryableAsync();

        if (input.CheckListId.HasValue)
        {
            queryable = queryable.Where(x => x.CheckListId == input.CheckListId.Value);
        }

        if (input.ListItemId.HasValue)
        {
            queryable = queryable.Where(x => x.ListItemId == input.ListItemId.Value);
        }

        var sorting = input.Sorting.IsNullOrWhiteSpace()
            ? $"{nameof(EntityAttachment.CreationTime)} desc"
            : input.Sorting;

        queryable = queryable.OrderBy(sorting);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var items = await AsyncExecuter.ToListAsync(
            queryable
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        var dtos = ObjectMapper.Map<System.Collections.Generic.List<EntityAttachment>, System.Collections.Generic.List<EntityAttachmentDto>>(items);
        return new PagedResultDto<EntityAttachmentDto>(totalCount, dtos);
    }

    public async Task<EntityAttachmentDto> UploadForCheckListAsync(Guid checkListId, IFormFile file)
    {
        Check.NotNull(file, nameof(file));

        using var stream = new System.IO.MemoryStream();
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

        return ObjectMapper.Map<EntityAttachment, EntityAttachmentDto>(entity);
    }

    public async Task<EntityAttachmentDto> UploadForListItemAsync(Guid listItemId, IFormFile file)
    {
        Check.NotNull(file, nameof(file));

        using var stream = new System.IO.MemoryStream();
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

        return ObjectMapper.Map<EntityAttachment, EntityAttachmentDto>(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);

        if (entity.Attachment != null && !string.IsNullOrWhiteSpace(entity.Attachment.Path))
        {
            await _fileManager.DeleteFileAsync(entity.Attachment);
        }

        await _repository.DeleteAsync(entity, autoSave: true);
    }
}
