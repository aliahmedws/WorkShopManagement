using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using WorkShopManagement.Utils.Exceptions;

namespace WorkShopManagement.EntityAttachments.FileAttachments;

public class BlobStorageOptions
{
    public required string BaseUrl { get; set; }
    public required string BaseDir { get; set; }
    public required string BasePath { get; set; }
    public required int TempRetentionHours { get; set; }
    public required int TempCleanupIntervalMinutes { get; set; }
}

public class ConfigureBlobStorageOptions(IConfiguration configuration) : IConfigureOptions<BlobStorageOptions>
{
    public void Configure(BlobStorageOptions options)
    {
        var section = configuration.GetSection("BlobStorageSettings");

        section.Bind(options);
        ValidateOptions(section.Key, options);
        options.BaseUrl = options.BaseUrl.TrimEnd('/');
        options.BaseDir = options.BaseDir.TrimEnd('/', '\\');
        options.BasePath = options.BasePath.TrimEnd('/', '\\');
    }

    static void ValidateOptions(string section, BlobStorageOptions options)
    {
        var missingFields = new List<string>();
        if (string.IsNullOrWhiteSpace(options.BaseUrl)) missingFields.Add(nameof(options.BaseUrl));
        if (string.IsNullOrWhiteSpace(options.BaseDir)) missingFields.Add(nameof(options.BaseDir));
        if (string.IsNullOrWhiteSpace(options.BasePath)) missingFields.Add(nameof(options.BasePath));
        if (options.TempRetentionHours <= 0) missingFields.Add(nameof(options.TempRetentionHours));
        if (options.TempCleanupIntervalMinutes <= 0) missingFields.Add(nameof(options.TempCleanupIntervalMinutes));
        if (missingFields.Count > 0)
        {
            var errors = new Dictionary<string, string[]>
                {
                    { section, [..missingFields] }
                };

            throw new MissingConfigurationsException(errors);
        }
    }
}
