using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.FileAttachments;

[RemoteService(IsEnabled = false)]
public class FileAttachmentAppService : ApplicationService, IFileAttachmentAppService
{
    private readonly FileAttachmentManager _manager;

    public FileAttachmentAppService(FileAttachmentManager manager)
    {
        _manager = manager;
    }

    public async Task SetAttachmentAsync(Guid entityId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new UserFriendlyException("File is required.");
        }

        await _manager.SetAttachmentAsync(entityId, file);
    }

    public async Task RemoveAttachmentAsync(Guid entityId)
    {
        await _manager.RemoveAttachmentAsync(entityId);
    }
}
