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
    private readonly IBlobContainer _container;
    private readonly GoogleStorageOptions _options;

    public TempFileManager(IOptions<GoogleStorageOptions> options, IBlobContainer container)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _container = container;
    }

    public async Task<FileAttachment> SaveAsync(MemoryStream fileStream, string originalFileName, CancellationToken ct = default)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new BusinessException(WorkShopManagementDomainErrorCodes.EmptyFile);
        }

        FileHelper.ValidateFileName(originalFileName);
        var ext = FileHelper.ValidateFileExtension(originalFileName);

        var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var id = Guid.NewGuid().ToString("N").ToUpperInvariant();

        var blobName = $"{ts}_{id}{ext}";

        await _container.SaveAsync(BuildBlobPath(blobName), fileStream, overrideExisting: true, cancellationToken: ct);

        var url = BuildBlobUrl(blobName);
        return new FileAttachment(originalFileName, blobName, url);
    }

    public async Task<Stream> GetAsync(string tempBlobName, CancellationToken ct = default)
    {
        FileHelper.ValidateFileName(tempBlobName);
        return await _container.GetAsync(BuildBlobPath(tempBlobName), cancellationToken: ct);
    }

    public async Task<byte[]> GetAllBytesAsync(string tempBlobName, CancellationToken ct = default)
    {
        FileHelper.ValidateFileName(tempBlobName);
        return await _container.GetAllBytesAsync(BuildBlobPath(tempBlobName), cancellationToken: ct);
    }

    public async Task<bool> DeleteAsync(string tempBlobName, CancellationToken ct = default)
    {
        FileHelper.ValidateFileNameWithExtension(tempBlobName);
        return await _container.DeleteAsync(BuildBlobPath(tempBlobName), cancellationToken: ct);
    }

    public string BuildBlobPath(string tempBlobName)
    {
        return _options.TempPrefix.EnsureEndsWith('/') + tempBlobName;
    }

    public string BuildBlobUrl(string tempBlobName)
    {
        return $"{_options.ContainerPath.EnsureEndsWith('/')}{BuildBlobPath(tempBlobName)}".Replace("\\", "/");
    }

    public async Task<int> CleanupOldFilesAsync(TimeSpan retention, CancellationToken ct = default)   // read retention from config later
    {
        var tempDir = _options.ContainerPath.EnsureEndsWith('/') + _options.TempPrefix;

        if (!Directory.Exists(tempDir))
        {
            // No directory = nothing to clean. This is not an error.
#pragma warning disable CA1873 // Avoid potentially expensive logging
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

            var tsParts = fileName[..underScoreIndex];
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
                bool deleted = await _container.DeleteAsync(fileName, cancellationToken: ct);
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