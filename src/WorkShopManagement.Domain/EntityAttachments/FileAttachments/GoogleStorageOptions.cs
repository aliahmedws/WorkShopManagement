using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkShopManagement.EntityAttachments.FileAttachments;

public class GoogleStorageOptions
{
    public const string ConfigurationKey = "Storage:Google";
    public string PublicBaseUrl { get; set; } = default!;
    public string ClientEmail { get; set; } = default!;
    public string ProjectId { get; set; } = default!;
    public string PrivateKey { get; set; } = default!;
    public List<string> Scopes { get; set; } = default!;
    public string ContainerName { get; set; } = default!;
    public string ContainerPath { get; set; } = default!;
    public string FilesPrefix { get; set; } = default!;
    public string TempPrefix { get; set; } = default!;
    public double TempRetentionHours { get; set; } = default!;
}

public class ConfigureGoogleStorageOptions(IConfiguration configuration) : IConfigureOptions<GoogleStorageOptions>
{
    private readonly IConfiguration _configuration = configuration;

    public void Configure(GoogleStorageOptions options)
    {
        _configuration.GetSection(GoogleStorageOptions.ConfigurationKey).Bind(options);
        ValidateOptions(options);

        options.PublicBaseUrl = options.PublicBaseUrl.TrimEnd('/');
        options.ContainerPath = options.PublicBaseUrl.EnsureEndsWith('/') + options.ContainerName.EnsureEndsWith('/') + "host";
    }

    static void ValidateOptions(GoogleStorageOptions options)
    {
        static void Require(string? value, string field)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new OptionsValidationException(
                    GoogleStorageOptions.ConfigurationKey,
                    typeof(GoogleStorageOptions),
                    [$"{field} is required."]);
            }
        }

        Require(options.PublicBaseUrl, nameof(options.PublicBaseUrl));
        Require(options.ClientEmail, nameof(options.ClientEmail));
        Require(options.ProjectId, nameof(options.ProjectId));
        Require(options.PrivateKey, nameof(options.PrivateKey));
        Require(options.ContainerName, nameof(options.ContainerName));

        if (options.Scopes is null || options.Scopes.Count == 0)
        {
            throw new OptionsValidationException(
                GoogleStorageOptions.ConfigurationKey,
                typeof(GoogleStorageOptions),
                [$"{nameof(options.Scopes)} is required and must contain at least one value."]);
        }

        if (options.Scopes.Any(s => string.IsNullOrWhiteSpace(s)))
        {
            throw new OptionsValidationException(
                GoogleStorageOptions.ConfigurationKey,
                typeof(GoogleStorageOptions),
                [$"{nameof(options.Scopes)} must not contain empty values."]);
        }

        // Retention must be > 0 (or adjust if you want to allow 0 to mean "disabled")
        if (options.TempRetentionHours <= 0)
        {
            throw new OptionsValidationException(
                GoogleStorageOptions.ConfigurationKey,
                typeof(GoogleStorageOptions),
                [$"{nameof(options.TempRetentionHours)} must be greater than 0."]);
        }
    }
}