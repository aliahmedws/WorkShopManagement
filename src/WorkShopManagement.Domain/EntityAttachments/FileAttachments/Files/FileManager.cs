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

public class FileManager : DomainService
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

    public async Task<FileAttachment> SaveAsync(MemoryStream fileStream, string fileName, string? vin, CancellationToken ct = default)
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

        await _container.SaveAsync(BuildBlobPath(blobName, vin), fileStream, overrideExisting: true, cancellationToken: ct);

        var filePath = BuildBlobUrl(blobName, vin);
        return new FileAttachment(fileName, filePath, blobName);

    }

    public async Task<FileAttachment> SaveFromTempAsync(string fileName, string tempBlobName, string? vin, CancellationToken cancellationToken = default)
    {
        FileHelper.ValidateFileNameWithExtension(fileName);
        FileHelper.ValidateFileNameWithExtension(tempBlobName);

        var fileStream = await _tempFileManager.GetAsync(tempBlobName, cancellationToken);

        await _container.SaveAsync(BuildBlobPath(tempBlobName, vin), fileStream, overrideExisting: true, cancellationToken: cancellationToken);
        var filePath = BuildBlobUrl(tempBlobName, vin);

        return new FileAttachment(fileName, tempBlobName, filePath);
    }

    public async Task<bool> DeleteAsync(FileAttachment attachment, string? vin)
    {
        ArgumentNullException.ThrowIfNull(attachment);
        FileHelper.ValidateFileNameWithExtension(attachment.BlobName);

        return await _container.DeleteAsync(BuildBlobPath(attachment.BlobName, vin));
    }

    public async Task<bool> DeleteAsync(string blobName, string? vin)
    {
        FileHelper.ValidateFileNameWithExtension(blobName);
        return await _container.DeleteAsync(BuildBlobPath(blobName, vin));
    }

    public async Task<byte[]> GetAllBytesAsync(string blobName, string? vin, CancellationToken cancellationToken = default)
    {
        FileHelper.ValidateFileName(blobName);
        return await _container.GetAllBytesAsync(BuildBlobPath(blobName, vin), cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetAsync(string blobName, string? vin, CancellationToken cancellationToken = default)
    {
        FileHelper.ValidateFileName(blobName);
        return await _container.GetAsync(BuildBlobPath(blobName, vin), cancellationToken: cancellationToken);
    }

    public string BuildBlobPath(string blobName, string? vin)
    {
        var path = _options.FilesPrefix.EnsureEndsWith('/');
        if (!string.IsNullOrWhiteSpace(vin))
        {
            path += vin.EnsureEndsWith('/');
        }
        return path + blobName;
    }

    private string BuildBlobUrl(string blobName, string? vin)
    {
        return $"{_options.ContainerPath}/{BuildBlobPath(blobName, vin)}".Replace("\\", "/");
    }
}