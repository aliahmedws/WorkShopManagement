using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.TempFiles
{
    public interface ITempFileAppService : IApplicationService
    {
        Task<List<TempFileDto>> UploadTempFilesAsync(List<IFormFile?> files);
    }
}
