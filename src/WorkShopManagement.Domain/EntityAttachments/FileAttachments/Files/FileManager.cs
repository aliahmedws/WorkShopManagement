using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Services;
using WorkShopManagement.EntityAttachments.FileAttachments.TempFiles;
using WorkShopManagement.Utils.Exceptions;

namespace WorkShopManagement.EntityAttachments.FileAttachments.Files;

public class FileManager(
    IBlobContainer<FileContainer> blobContainer,
    IConfiguration configuration,
    TempFileManager tempFileManager
) : DomainService
{
    private readonly IBlobContainer<FileContainer> _blobContainer = blobContainer;
    private readonly IConfiguration _configuration = configuration;
    private readonly TempFileManager _tempFileManager = tempFileManager;


    public async Task<FileAttachment> SaveAsync(MemoryStream fileStream, string fileName, string? folder = null, CancellationToken ct = default)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new BusinessException(WorkShopManagementDomainErrorCodes.EmptyFile);
        }
        FileHelper.ValidateFileName(fileName);
        var fileExt = FileHelper.ValidateFileExtension(fileName);

        var blobName = $"{Guid.NewGuid():N}{fileExt}";
        var normalizedFolder = FileHelper.NormalizePath(folder);

        if (!string.IsNullOrEmpty(normalizedFolder))
        {
            blobName = $"{normalizedFolder}/{blobName}";
        }

        await _blobContainer.SaveAsync(blobName, fileStream, overrideExisting: true, cancellationToken: ct);

        var filePath = BuildUrl(blobName);
        return new FileAttachment(fileName, filePath, blobName);

    }

    public async Task<FileAttachment> SaveFromTempAsync(string fileName, string tempBlobName, string? folder = null, CancellationToken cancellationToken = default)
    {
        FileHelper.ValidateFileNameWithExtension(fileName);
        FileHelper.ValidateFileNameWithExtension(tempBlobName);

        var fileStream = await _tempFileManager.GetAsync(tempBlobName, cancellationToken);

        var blobName = tempBlobName;                //save with same name, can be changed to new name if needed

        var normalizedFolder = FileHelper.NormalizePath(folder);
        if (!string.IsNullOrEmpty(normalizedFolder))
        {
            blobName = $"{normalizedFolder}/{blobName}";
        }

        await _blobContainer.SaveAsync(blobName, fileStream, overrideExisting: true, cancellationToken: cancellationToken);
        var filePath = BuildUrl(blobName);

        //await _tempFileManager.DeleteAsync(fileName); // Optionally delete the temp file after saving

        return new FileAttachment(fileName, blobName, filePath);
    }



    public async Task DeleteAsync(FileAttachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);
        FileHelper.ValidateFileNameWithExtension(attachment.BlobName);

        await _blobContainer.DeleteAsync(attachment.BlobName);
    }

    public async Task DeleteAsync(string blobName)
    {

        FileHelper.ValidateFileNameWithExtension(blobName);
        await _blobContainer.DeleteAsync(blobName);
    }

    public async Task<byte[]> GetAllBytesAsync(string blobName, CancellationToken cancellationToken = default)
    {
        FileHelper.ValidateFileName(blobName);
        return await _blobContainer.GetAllBytesAsync(blobName, cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetAsync(string blobName, CancellationToken cancellationToken = default)
    {
        FileHelper.ValidateFileName(blobName);
        return await _blobContainer.GetAsync(blobName, cancellationToken: cancellationToken);
    }

    private string BuildUrl(string blobName)
    {
        var containerName = BlobContainerNameAttribute.GetContainerName<FileContainer>();
        var baseUrl = _configuration["BlobStorageSettings:BaseUrl"];
        var basePath = _configuration["BlobStorageSettings:BasePath"];
        List<string> missingFields = [];
        if (string.IsNullOrWhiteSpace(baseUrl))
            missingFields.Add("BlobStorageSettings:BaseUrl");
        if (string.IsNullOrWhiteSpace(basePath))
            missingFields.Add("BlobStorageSettings:BasePath");
        if (missingFields.Count > 0)
        {
            throw new MissingConfigurationsException([.. missingFields]);
        }

        baseUrl = baseUrl!.TrimEnd('/');
        basePath = basePath!.Trim('/');

        return $"{baseUrl}/{basePath}/{containerName}/{blobName}".Replace("\\", "/");

    }

}



