using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace WorkShopManagement.FileAttachments;

[RemoteService(IsEnabled = true)]
[ControllerName("FileAttachments")]
[Area("app")]
[Route("api/app/file-attachments")]
public class FileAttachmentController : AbpController, IFileAttachmentAppService
{
    private readonly IFileAttachmentAppService _fileAttachmentAppService;

    public FileAttachmentController(IFileAttachmentAppService fileAttachmentAppService)
    {
        _fileAttachmentAppService = fileAttachmentAppService;
    }

    [HttpDelete("{entityId}")]
    public async Task RemoveAttachmentAsync(Guid entityId)
    {
        await _fileAttachmentAppService.RemoveAttachmentAsync(entityId);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("upload")]
    public async Task SetAttachmentAsync([FromForm] Guid entityId, [FromForm] IFormFile file)
    {
        await _fileAttachmentAppService.SetAttachmentAsync(entityId, file);
    }
}
