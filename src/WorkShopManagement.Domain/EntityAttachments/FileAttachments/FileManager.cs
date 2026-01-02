using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;
using Volo.Abp.MultiTenancy;
using WorkShopManagement.Data;

namespace WorkShopManagement.EntityAttachments.FileAttachments;

public class FileManager(
    IBlobContainer<FileContainer> blobContainer,
    IConfiguration configuration,
    IBlobContainer<TempFileContainer> tempContainer
) : DomainService
{
    private readonly IBlobContainer<FileContainer> _blobContainer = blobContainer;
    private readonly IBlobContainer<TempFileContainer> _tempContainer = tempContainer;
    private readonly IConfiguration _configuration = configuration;


    public async Task<FileAttachment> SaveFileAsync(string fileName, string? folder = null, CancellationToken cancellationToken = default)
    {
        var stream = await GetTempFileAsync(fileName, cancellationToken);
        if (stream == null)
            throw new BusinessException(WorkShopManagementDomainErrorCodes.EmptyFile)
                .WithData("fileName", fileName);

        if(!string.IsNullOrEmpty(folder?.Trim('/').Replace("\\", "/")))
        {
            fileName = $"{folder}/{fileName}";
        }

        await _blobContainer.SaveAsync(fileName, stream, overrideExisting: true, cancellationToken: cancellationToken);
        var filePath = BuildUrl(fileName);

        //await DeleteTempFileAsync(fileName); // Optionally delete the temp file after saving

        return new FileAttachment(fileName, filePath);
    }

    private string BuildUrl(string fileName)
    {
        var containerName = BlobContainerNameAttribute.GetContainerName<FileContainer>();
        var baseUrl = _configuration["BlobStorageSettings:BaseUrl"]!.TrimEnd('/');
        var basePath = _configuration["BlobStorageSettings:BasePath"]!.Trim('/');

        return $"{baseUrl}/{basePath}/{containerName}/{fileName}".Replace("\\", "/");

    }


    public async Task DeleteFileAsync(FileAttachment attachment)
    {
        if (attachment == null)
            throw new BusinessException("FileAttachment cannot be null.");

        await _blobContainer.DeleteAsync(attachment.Name);


        // If you still keep wwwroot file mirrors, delete that physical file too
        // (but with ABP FileSystemBlobProvider, blob is already on disk; you often don't need this extra deletion)

        //var baseUrl = _configuration["LocalStorageSetting:BaseUrl"] ?? "https://localhost:44322/";
        //var relativeUrl = attachment.Path.Replace(baseUrl.TrimEnd('/') + "/", "");

        //var fullPath = Path.Combine(Environment.CurrentDirectory, "wwwroot",
        //    relativeUrl.Replace("/", Path.DirectorySeparatorChar.ToString()));

        //if (File.Exists(fullPath))
        //{
        //    File.Delete(fullPath);
        //}
    }

    public async Task<byte[]> GetAllBytesAsync(string fileName, CancellationToken cancellationToken = default)
    {
        ValidateFileName(fileName);
        return await _blobContainer.GetAllBytesAsync(fileName, cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetAsync(string fileName, CancellationToken cancellationToken = default)
    {
        ValidateFileName(fileName);
        return await _blobContainer.GetAsync(fileName, cancellationToken: cancellationToken);
    }




    private static void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new BusinessException(WorkShopManagementDomainErrorCodes.NullField)
                .WithData("field", nameof(fileName));
        }

        var ext = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(ext))
        {
            throw new BusinessException(WorkShopManagementDomainErrorCodes.InvalidFileFormat);
        }
    }


    // TEMP FILE METHODS
    public async Task<(string, string)> SaveTempFileAsync(MemoryStream fileStream, string fileName, string? folder = null, CancellationToken ct = default)
    {
        if (fileStream == null || fileStream.Length == 0)
            throw new BusinessException(WorkShopManagementDomainErrorCodes.EmptyFile);

        ValidateFileName(fileName);
        var fileExt = Path.GetExtension(fileName);

        var blobName = $"{Guid.NewGuid()}{fileExt}";

        if (!string.IsNullOrEmpty(folder?.Trim('/').Replace("\\", "/")))
        {
            blobName = $"{folder}/{Guid.NewGuid()}{fileExt}";
        }

        await _tempContainer.SaveAsync(blobName, fileStream, overrideExisting: true, cancellationToken: ct);

        var containerName = BlobContainerNameAttribute.GetContainerName<TempFileContainer>();
        var baseUrl = _configuration["BlobStorageSettings:BaseUrl"]!.TrimEnd('/');
        var basePath = _configuration["BlobStorageSettings:BasePath"]!.Trim('/');

        var blobPath = $"{baseUrl}/{basePath}//{containerName}/{blobName}".Replace("\\", "/");

        return (blobName, blobPath);

    }

    public async Task<Stream> GetTempFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        ValidateFileName(fileName);
        return await _tempContainer.GetAsync(fileName, cancellationToken: cancellationToken);
    }

    public async Task DeleteTempFileAsync(string fileName)
    {
        if(string.IsNullOrWhiteSpace(fileName))
            throw new BusinessException("Blob name cannot be null or empty.");

        await _blobContainer.DeleteAsync(fileName);
    }

    
}

