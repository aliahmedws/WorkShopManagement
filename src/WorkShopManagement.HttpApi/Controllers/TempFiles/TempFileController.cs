using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.TempFiles;

namespace WorkShopManagement.Controllers.TempFiles
{
    [RemoteService(IsEnabled = true)]
    [ControllerName("TempFile")]
    [Area("app")]
    [Route("api/app/temp-file")]
    public class TempFileController(ITempFileAppService tempFileAppService) : WorkShopManagementController, ITempFileAppService
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("upload")]
        public Task<List<FileAttachmentDto>> UploadTempFilesAsync(List<IFormFile?> files)
        {
            return tempFileAppService.UploadTempFilesAsync(files);
        }
    }
}
