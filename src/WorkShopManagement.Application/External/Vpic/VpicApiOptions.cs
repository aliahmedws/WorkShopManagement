using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using Volo.Abp;
using WorkShopManagement.External.Shared;

namespace WorkShopManagement.External.Vpic;

public class VpicApiOptions
{
    public const string ConfigurationKey = "External:Vpic";
    public string BaseUrl { get; set; } = default!;
    public string DecodeVinExtendedUrl { get; set; } = default!;
}

public class ConfigureVpicApiOptions(IConfiguration configuration) : IConfigureOptions<VpicApiOptions>
{
    private readonly IConfiguration _configuration = configuration;

    public void Configure(VpicApiOptions options)
    {
        _configuration.GetSection(VpicApiOptions.ConfigurationKey).Bind(options);
        options.BaseUrl = options.BaseUrl.TrimEnd('/');
    }
}

public static class VpicApiOptionsExtensions
{
    public static IRestClient CreateRestClient(this VpicApiOptions options, IRestClientFactory restClientFactory)
    {
        return restClientFactory.Create(options.BaseUrl, CreateRestClientProfile(options));
    }

    private static RestClientProfile CreateRestClientProfile(VpicApiOptions options)
    {
        return new RestClientProfile()
        {
            DefaultQueryParams = new Dictionary<string, string>()
            {
                { "format", "json" }
            }
        };
    }

    public static RestRequest CreateDecodeVinExtendedRequest(this VpicApiOptions options, string vin, string? modelYear = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(vin);

        modelYear = modelYear?.Trim();

        if (string.IsNullOrWhiteSpace(vin))
        {
            throw new UserFriendlyException("VIN is required.");
        }

        if (vin.Length != 17)
        {
            throw new UserFriendlyException("VIN must be exactly 17 characters.");
        }

        if (!modelYear.IsNullOrWhiteSpace() && !int.TryParse(modelYear, out _))
        {
            throw new UserFriendlyException("Model year must be a valid year (e.g., 2018).");
        }

        var encodedVin = Uri.EscapeDataString(vin);
        var path = string.Format(options.DecodeVinExtendedUrl, encodedVin);

        var request = new RestRequest(path, Method.Get);

        if (!modelYear.IsNullOrWhiteSpace())
        {
            request.AddQueryParameter("modelyear", modelYear.Trim());
        }

        return request;
    }
}
