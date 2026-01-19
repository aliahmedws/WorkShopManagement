using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.Storage;

[RemoteService(false)]
[Authorize]
public class SignedUrlAppService(IOptions<GoogleStorageOptions> options) : WorkShopManagementAppService, ISignedUrlAppService
{
    private readonly GoogleStorageOptions _options = options.Value;

    public Task<string> GetSignedReadUrlAsync(string gcsUriOrObjectName)
    {
        var (bucket, obj) = ParseBucketAndObject(gcsUriOrObjectName);

        EnsureAllowedBucketAndPrefix(bucket, obj);

        var signedUrl = SignedUrlFactory.CreateGetSignedUrl(
            bucketName: bucket,
            objectName: obj,
            clientEmail: _options.ClientEmail,
            privateKeyPem: _options.PrivateKey,
            lifetime: TimeSpan.FromMinutes(15)
            );

        return Task.FromResult(signedUrl);
    }

    private (string bucket, string obj) ParseBucketAndObject(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new UserFriendlyException("Object path is required.");

        input = input.Trim();

        // 1) gs://bucket/object
        if (input.StartsWith("gs://", StringComparison.OrdinalIgnoreCase))
        {
            var rest = input["gs://".Length..];
            var idx = rest.IndexOf('/');
            if (idx <= 0 || idx >= rest.Length - 1)
                throw new UserFriendlyException("Invalid gs:// URI.");

            return (rest[..idx], rest[(idx + 1)..]);
        }

        // 2) https://storage.googleapis.com/bucket/object
        // 3) https://bucket.storage.googleapis.com/object
        if (Uri.TryCreate(input, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp))
        {
            var host = uri.Host;                 // e.g. storage.googleapis.com OR wsm-abp-blobstorage-files.storage.googleapis.com
            var path = uri.AbsolutePath.Trim('/'); // e.g. wsm-abp-blobstorage-files/host/temp/abc.png OR host/temp/abc.png

            if (host.Equals("storage.googleapis.com", StringComparison.OrdinalIgnoreCase))
            {
                // path starts with "{bucket}/..."
                var idx = path.IndexOf('/');
                if (idx <= 0 || idx >= path.Length - 1)
                    throw new UserFriendlyException("Invalid Google Cloud Storage URL.");

                var bucket = path[..idx];
                var obj = path[(idx + 1)..];
                return (bucket, obj);
            }

            const string suffix = ".storage.googleapis.com";
            if (host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                // host is "{bucket}.storage.googleapis.com"
                var bucket = host[..^suffix.Length];
                if (string.IsNullOrWhiteSpace(bucket) || string.IsNullOrWhiteSpace(path))
                    throw new UserFriendlyException("Invalid Google Cloud Storage URL.");

                return (bucket, path);
            }

            // If it's some other host, reject. Do not sign arbitrary URLs.
            throw new UserFriendlyException("Only Google Cloud Storage URLs are supported.");
        }

        // 4) Bare objectName (recommended)
        return (_options.ContainerName, input.TrimStart('/'));
    }

    private void EnsureAllowedBucketAndPrefix(string bucket, string obj)
    {
        if (!string.Equals(bucket, _options.ContainerName, StringComparison.OrdinalIgnoreCase))
            throw new UserFriendlyException("Invalid bucket.");

        EnsureAllowedPrefix(obj);
    }

    private void EnsureAllowedPrefix(string objectName)
    {
        var normalized = objectName.TrimStart('/');

        var files = _options.FilesPrefix.Trim('/'); // e.g. "files"
        var temp = _options.TempPrefix.Trim('/');  // e.g. "temp"

        bool allowed =
            normalized.StartsWith(files + "/", StringComparison.OrdinalIgnoreCase) ||
            normalized.StartsWith(temp + "/", StringComparison.OrdinalIgnoreCase) ||
            normalized.StartsWith("host/" + files + "/", StringComparison.OrdinalIgnoreCase) ||
            normalized.StartsWith("host/" + temp + "/", StringComparison.OrdinalIgnoreCase);

        if (!allowed)
            throw new UserFriendlyException("Invalid object path.");
    }
}