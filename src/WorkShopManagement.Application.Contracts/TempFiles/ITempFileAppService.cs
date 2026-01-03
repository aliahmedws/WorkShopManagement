using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.TempFiles
{
    public interface ITempFileAppService : IApplicationService
    {
        Task<List<FileAttachmentDto>> UploadTempFilesAsync(List<IFormFile?> files);
    }
}
