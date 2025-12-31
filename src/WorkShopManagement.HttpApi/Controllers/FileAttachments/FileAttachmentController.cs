using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using WorkShopManagement.FileAttachments;

namespace WorkShopManagement.Controllers.FileAttachments;

[RemoteService(IsEnabled = true)]
[ControllerName("FileAttachments")]
[Area("app")]
[Route("api/app/file-attachments")]
public class FileAttachmentController : AbpController, IEntityAttachmentAppService
{
    private readonly IEntityAttachmentAppService _appService;

    public FileAttachmentController(IEntityAttachmentAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<EntityAttachmentDto>> GetListAsync(GetEntityAttachmentListDto input)
        => _appService.GetListAsync(input);

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("upload/check-list")]
    public Task<EntityAttachmentDto> UploadForCheckListAsync([FromForm] Guid checkListId, [FromForm] IFormFile file)
        => _appService.UploadForCheckListAsync(checkListId, file);

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("upload/list-item")]
    public Task<EntityAttachmentDto> UploadForListItemAsync([FromForm] Guid listItemId, [FromForm] IFormFile file)
        => _appService.UploadForListItemAsync(listItemId, file);

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
        => _appService.DeleteAsync(id);
}
