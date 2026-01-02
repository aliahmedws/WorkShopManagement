using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WorkShopManagement.Data;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.TempFiles
{
    public class TempFileAppService(
        FileManager fileManager) : WorkShopManagementAppService, ITempFileAppService
    {
        private readonly FileManager _fileManager = fileManager;

        public async Task<List<TempFileDto>> UploadTempFilesAsync(List<IFormFile?> files)
        {
            var tempFiles = new List<TempFileDto>();
            if (files != null && files.Count != 0)
            {
                foreach (var f in files)
                {
                    if (f != null)
                    {
                        using var stream = new MemoryStream();
                        await f.CopyToAsync(stream);
                        stream.Position = 0;
                        var (name, path) = await _fileManager.SaveTempFileAsync(stream, f.FileName);
                        tempFiles.Add(new TempFileDto { Name = name, Path = path });
                    }

                }
            }

            return tempFiles;
        }
    }
}
