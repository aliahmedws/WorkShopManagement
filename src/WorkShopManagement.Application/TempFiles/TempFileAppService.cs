using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments.TempFiles;

namespace WorkShopManagement.TempFiles
{
    public class TempFileAppService(
        TempFileManager tempManager) : WorkShopManagementAppService, ITempFileAppService
    {
        private readonly TempFileManager _tempFileManager = tempManager;

        public async Task<List<FileAttachmentDto>> UploadTempFilesAsync(List<IFormFile?> files)
        {
            var tempFiles = new List<FileAttachment>();
            if (files != null && files.Count != 0)
            {
                foreach (var f in files)
                {
                    if (f != null)
                    {
                        using var stream = new MemoryStream();
                        await f.CopyToAsync(stream);
                        stream.Position = 0;
                        var file = await _tempFileManager.SaveAsync(stream, f.FileName);
                        tempFiles.Add(file);
                    }

                }
            }

            return ObjectMapper.Map<List<FileAttachment>, List<FileAttachmentDto>>(tempFiles);
        }
    }
}
