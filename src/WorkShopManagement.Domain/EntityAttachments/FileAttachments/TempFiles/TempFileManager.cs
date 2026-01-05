using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Services;
using WorkShopManagement.Utils.Exceptions;

namespace WorkShopManagement.EntityAttachments.FileAttachments.TempFiles;

public class TempFileManager(
    IBlobContainer<TempFileContainer> tempContainer,
    IConfiguration configuration
) : DomainService
{
    private readonly IBlobContainer<TempFileContainer> _tempContainer = tempContainer;
    private readonly IConfiguration _configuration = configuration;

    public async Task<FileAttachment> SaveAsync(
        MemoryStream fileStream,
        string originalFileName,
        string? folder = null,
        CancellationToken ct = default)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new BusinessException(WorkShopManagementDomainErrorCodes.EmptyFile);
        }
        FileHelper.ValidateFileName(originalFileName);

        var ext = FileHelper.ValidateFileExtension(originalFileName);

        var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var id = Guid.NewGuid().ToString("N");

        var blobName = $"{ts}_{id}{ext}";

        var normalizedFolder = FileHelper.NormalizePath(folder);
        if (!string.IsNullOrEmpty(normalizedFolder))
            blobName = $"{normalizedFolder}/{blobName}";

        await _tempContainer.SaveAsync(blobName, fileStream, overrideExisting: true, cancellationToken: ct);

        var url = BuildUrl(blobName);
        return new FileAttachment(originalFileName, blobName, url);
    }

    public async Task<Stream> GetAsync(string tempBlobName, CancellationToken ct = default)
    {
        FileHelper.ValidateFileName(tempBlobName);
        return await _tempContainer.GetAsync(tempBlobName, cancellationToken: ct);
       
    }

    public async Task<byte[]> GetAllBytesAsync(string tempBlobName, CancellationToken ct = default)
    {
        FileHelper.ValidateFileName(tempBlobName);
        return await _tempContainer.GetAllBytesAsync(tempBlobName, cancellationToken: ct);
    }

    public async Task DeleteAsync(string tempBlobName, CancellationToken ct = default)
    {
        FileHelper.ValidateFileNameWithExtension(tempBlobName);

        await _tempContainer.DeleteAsync(tempBlobName, cancellationToken: ct);
    }

    public string BuildUrl(string tempBlobName)
    {
        var containerName = BlobContainerNameAttribute.GetContainerName<TempFileContainer>();

        var baseUrlRaw = _configuration["BlobStorageSettings:BaseUrl"];
        var basePathRaw = _configuration["BlobStorageSettings:BasePath"];

        var missingFields = new List<string>();
        if (string.IsNullOrWhiteSpace(baseUrlRaw)) missingFields.Add("BlobStorageSettings:BaseUrl");
        if (string.IsNullOrWhiteSpace(basePathRaw)) missingFields.Add("BlobStorageSettings:BasePath");

        if (missingFields.Count > 0)
            throw new MissingConfigurationsException([.. missingFields]);

        var baseUrl = baseUrlRaw!.TrimEnd('/');
        var basePath = basePathRaw!.Trim('/');

        return $"{baseUrl}/{basePath}/{containerName}/{tempBlobName}".Replace("\\", "/");
    }
}
