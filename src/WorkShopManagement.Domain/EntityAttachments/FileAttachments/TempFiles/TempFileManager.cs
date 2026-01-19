using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
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

    public async Task<int> CleanupOldFilesAsync(TimeSpan retention, CancellationToken ct = default)
    {
        var bucket = _options.ContainerName;

        var prefix = "host/" + _options.TempPrefix.EnsureEndsWith('/');

        var cutoffUtc = DateTime.UtcNow - retention;

        var deletedCount = 0;
        var scannedCount = 0;
        var parseFailedCount = 0;

        var values = new Dictionary<string, string>
            {
                { "type", "service_account" },
                { "client_email", _options.ClientEmail },
                { "private_key", _options.PrivateKey }
            };

        var credential = GoogleCredential.FromJson(JsonSerializer.Serialize(values));

        if (credential.UnderlyingCredential is not ServiceAccountCredential saCred)
            throw new InvalidOperationException("Invalid service account credentials. Check ClientEmail/PrivateKey.");

        var storage = StorageClient.Create(credential);
        var storageObjects = storage.ListObjects(_options.ContainerName, prefix);


        foreach (var obj in storageObjects)
        {
            ct.ThrowIfCancellationRequested();
            scannedCount++;

            var objectName = obj.Name;
            if (string.IsNullOrWhiteSpace(objectName) || !objectName.StartsWith(prefix, StringComparison.Ordinal))
            {
                parseFailedCount++;
                continue;
            }
            var fileNameOnly = objectName[prefix.Length..];

            var underScoreIndex = fileNameOnly.IndexOf('_');
            if (underScoreIndex <= 0)
            {
                parseFailedCount++;
                Logger.LogWarning("TempFileManager.CleanupOldFiles: Skipping object with invalid name (no timestamp): {ObjectName}", objectName);
                continue;
            }

            var tsPart = fileNameOnly[..underScoreIndex];
            if (!TryParseTimestamp(tsPart, out var fileTsUtc))
            {
                parseFailedCount++;
                Logger.LogWarning("TempFileManager.CleanupOldFiles: Skipping object with invalid timestamp: {ObjectName}", objectName);
                continue;
            }

            if (fileTsUtc > cutoffUtc)
            {
                continue;
            }

            try
            {
                await storage.DeleteObjectAsync(bucket, objectName, cancellationToken: ct);
                deletedCount++;
            }
            catch (Google.GoogleApiException ex) when (ex.Error?.Code == 404)
            {
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "TempFileManager.CleanupOldFiles: Failed to delete temp object: {ObjectName}", objectName);
            }
        }

#pragma warning disable CA1873 // Avoid potentially expensive logging
        Logger.LogInformation(
            "TempFileManager.CleanupOldFiles: Completed. Scanned={Scanned}, Deleted={Deleted}, ParseFailed={ParseFailed}, CutoffUtc={CutoffUtc}, Bucket={Bucket}, Prefix={Prefix}",
            scannedCount, deletedCount, parseFailedCount, cutoffUtc, bucket, prefix
        );
#pragma warning restore CA1873 // Avoid potentially expensive logging

        return deletedCount;
    }
    public async Task<int> CleanupOldFilesAsync(CancellationToken ct = default)  
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
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out utc
        );
    }
}