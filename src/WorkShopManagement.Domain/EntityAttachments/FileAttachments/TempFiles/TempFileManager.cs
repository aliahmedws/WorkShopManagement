using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Services;

namespace WorkShopManagement.EntityAttachments.FileAttachments.TempFiles;

public class TempFileManager : DomainService
{
    private readonly IBlobContainer<TempFileContainer> _tempContainer;
    private readonly BlobStorageOptions _options;

    public TempFileManager(
        IBlobContainer<TempFileContainer> tempContainer,
        IOptions<BlobStorageOptions> options
    )
    {
        _tempContainer = tempContainer;
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<FileAttachment> SaveAsync(
        MemoryStream fileStream,
        string originalFileName,
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


        await _tempContainer.SaveAsync(blobName, fileStream, overrideExisting: true, cancellationToken: ct);

        var url = BuildBlobUrl(blobName);
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

    public string BuildBlobUrl(string tempBlobName)
    {
        var containerName = BlobContainerNameAttribute.GetContainerName<TempFileContainer>();
        var test = GetContainerPath();
        var baseUrl = _options.BaseUrl;
        var basePath = _options.BasePath;
        return $"{baseUrl}/{basePath}/{containerName}/{tempBlobName}".Replace("\\", "/");
    }

    public string GetContainerPath()
    {
        var containerName = BlobContainerNameAttribute.GetContainerName<TempFileContainer>();
        var basePath = _options.BasePath;
        var fullPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", basePath, containerName);

        return Path.GetFullPath(fullPath);
    }

    public async Task<int> CleanupOldFilesAsync(TimeSpan retention, CancellationToken ct = default)   // read retention from config later
    {
        var tempDir = GetContainerPath();

        if (!Directory.Exists(tempDir))
        {
            // No directory = nothing to clean. This is not an error.
            Logger.LogDebug("TempFileManager.CleanupOldFiles:Temp directory not found: {TempDir}", tempDir);
            return 0;
        }

        var cutoffUtc = DateTime.UtcNow - retention;

        var files = Directory.EnumerateFiles(tempDir, "*", SearchOption.TopDirectoryOnly);

        var deletedCount = 0;
        var scannedCount = 0;
        var parseFailedCount = 0;

        foreach (var file in files)
        {
            ct.ThrowIfCancellationRequested();
            scannedCount++;
            var fileName = Path.GetFileName(file);

            var underScoreIndex = fileName.IndexOf('_');
            if (underScoreIndex <= 0)
            {
                parseFailedCount++;
                Logger.LogWarning("TempFileManager.CleanupOldFiles: Skipping file with invalid name (no timestamp): {FileName}", fileName);
                continue;
            }

            var tsParts = fileName.Substring(0, underScoreIndex);
            if (!TryParseTimestamp(tsParts, out var fileTsUtc))
            {
                parseFailedCount++;
                Logger.LogWarning("TempFileManager.CleanupOldFiles: Skipping file with invalid timestamp: {FileName}", fileName);
                continue;
            }

            if (fileTsUtc.Date > cutoffUtc.Date)
            {
                continue; // not old enough
            }

            try
            {
                bool deleted = await _tempContainer.DeleteAsync(fileName, cancellationToken: ct);
                deletedCount++;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "TempFileManager.CleanupOldFiles: Failed to delete temp file blob: {FileName}", fileName);
            }

        }
        Logger.LogInformation(
            "TempFileManager.CleanupOldFiles: Temp cleanup completed. Scanned={Scanned}, Deleted={Deleted}, ParseFailed={ParseFailed}, CutoffUtc={CutoffUtc}, TempDir={TempDir}",
            scannedCount, deletedCount, parseFailedCount, cutoffUtc, tempDir
            );

        return deletedCount;
    }
    public async Task<int> CleanupOldFilesAsync(CancellationToken ct = default)   // read retention from config later
    {
        var retenion = TimeSpan.FromHours(_options.TempRetentionHours);

        return await CleanupOldFilesAsync(retenion, ct);

    }

    private static bool TryParseTimestamp(string ts, out DateTime utc)
    {
        return DateTime.TryParseExact(
            ts,
            "yyyyMMddHHmmssfff",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
            out utc
        );
    }
}
