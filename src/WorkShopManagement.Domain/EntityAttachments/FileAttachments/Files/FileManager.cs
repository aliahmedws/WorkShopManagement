using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Services;
using WorkShopManagement.EntityAttachments.FileAttachments.TempFiles;

namespace WorkShopManagement.EntityAttachments.FileAttachments.Files;

public class FileManager: DomainService
{
    private readonly IBlobContainer _container;
    private readonly TempFileManager _tempFileManager;
    private readonly GoogleStorageOptions _options;

    public FileManager(
        IBlobContainer container,
        IOptions<GoogleStorageOptions> options,
        TempFileManager tempFileManager
        )
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _container = container;
        _tempFileManager = tempFileManager;
    }

    public async Task<FileAttachment> SaveAsync(MemoryStream fileStream, string fileName, CancellationToken ct = default)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new BusinessException(WorkShopManagementDomainErrorCodes.EmptyFile);
        }
        FileHelper.ValidateFileName(fileName);
        var ext = FileHelper.ValidateFileExtension(fileName);

        var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var id = Guid.NewGuid().ToString("N").ToUpperInvariant();

        var blobName = $"{ts}_{id}{ext}";

        await _container.SaveAsync(BuildBlobPath(blobName), fileStream, overrideExisting: true, cancellationToken: ct);

        var filePath = BuildBlobUrl(blobName);
        return new FileAttachment(fileName, filePath, blobName);

    }

    public async Task<FileAttachment> SaveFromTempAsync(string fileName, string tempBlobName, CancellationToken cancellationToken = default)
    {
        FileHelper.ValidateFileNameWithExtension(fileName);
        FileHelper.ValidateFileNameWithExtension(tempBlobName);

        var fileStream = await _tempFileManager.GetAsync(tempBlobName, cancellationToken);

        await _container.SaveAsync(BuildBlobPath(tempBlobName), fileStream, overrideExisting: true, cancellationToken: cancellationToken);
        var filePath = BuildBlobUrl(tempBlobName);

        return new FileAttachment(fileName, tempBlobName, filePath);
    }

    public async Task<bool> DeleteAsync(FileAttachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);
        FileHelper.ValidateFileNameWithExtension(attachment.BlobName);

        return await _container.DeleteAsync(BuildBlobPath(attachment.BlobName));
    }

    public async Task<bool> DeleteAsync(string blobName)
    {
        FileHelper.ValidateFileNameWithExtension(blobName);
        return await _container.DeleteAsync(BuildBlobPath(blobName));
    }

    public async Task<byte[]> GetAllBytesAsync(string blobName, CancellationToken cancellationToken = default)
    {
        FileHelper.ValidateFileName(blobName);
        return await _container.GetAllBytesAsync(BuildBlobPath(blobName), cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetAsync(string blobName, CancellationToken cancellationToken = default)
    {
        FileHelper.ValidateFileName(blobName);
        return await _container.GetAsync(BuildBlobPath(blobName), cancellationToken: cancellationToken);
    }

    public string BuildBlobPath(string blobName)
    {
        return _options.FilesPrefix.EnsureEndsWith('/') + blobName;
    }

    private string BuildBlobUrl(string blobName)
    {
        return $"{_options.ContainerPath}/{BuildBlobPath(blobName)}".Replace("\\", "/");
    }
}