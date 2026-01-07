using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using WorkShopManagement.External.Shared;

namespace WorkShopManagement.External.CarsXE
{
    public class CarsXeApiOptions
    {
        public const string ConfigurationKey = "External:CarsXe";

        public string BaseUrl { get; set; } = default!;

        // Example values:
        // "international-vin-decoder"
        // "recalls"
        public string VinDecoderUrl { get; set; } = default!;
        public string RecallsUrl { get; set; } = default!;

        // API key (kept in appsettings/secret store)
        public string ApiKey { get; set; } = default!;
    }

    public class ConfigureCarsXeApiOptions(IConfiguration configuration) : IConfigureOptions<CarsXeApiOptions>
    {
        private readonly IConfiguration _configuration = configuration;

        public void Configure(CarsXeApiOptions options)
        {
            _configuration.GetSection(CarsXeApiOptions.ConfigurationKey).Bind(options);

            options.BaseUrl = (options.BaseUrl ?? string.Empty).Trim().TrimEnd('/');
            options.VinDecoderUrl = (options.VinDecoderUrl ?? string.Empty).Trim().TrimStart('/').TrimEnd('/');
            options.RecallsUrl = (options.RecallsUrl ?? string.Empty).Trim().TrimStart('/').TrimEnd('/');

            if (options.BaseUrl.IsNullOrWhiteSpace())
                throw new AbpException($"{CarsXeApiOptions.ConfigurationKey}:BaseUrl is missing.");

            if (options.ApiKey.IsNullOrWhiteSpace())
                throw new AbpException($"{CarsXeApiOptions.ConfigurationKey}:ApiKey is missing.");

            if (options.VinDecoderUrl.IsNullOrWhiteSpace())
                throw new AbpException($"{CarsXeApiOptions.ConfigurationKey}:VinDecoderUrl is missing.");

            if (options.RecallsUrl.IsNullOrWhiteSpace())
                throw new AbpException($"{CarsXeApiOptions.ConfigurationKey}:RecallsUrl is missing.");
        }
    }

    // -------------------- Extensions --------------------

    public static class CarsXeApiOptionsExtensions
    {
        public static IRestClient CreateRestClient(this CarsXeApiOptions options, IRestClientFactory restClientFactory)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(restClientFactory);

            // BaseUrl is already normalized in ConfigureCarsXeApiOptions
            return restClientFactory.Create(options.BaseUrl, CreateRestClientProfile(options));
        }

        private static RestClientProfile CreateRestClientProfile(CarsXeApiOptions options)
        {
            return new RestClientProfile
            {
                DefaultQueryParams = new Dictionary<string, string>
                {
                    // CarsXE expects `key` and `vin` as query params.
                    // Put API key here so you don't repeat it in every request.
                    ["key"] = options.ApiKey
                }
            };
        }

        public static RestRequest CreateVinDecoderRequest(this CarsXeApiOptions options, string vin)
        {
            ArgumentNullException.ThrowIfNull(options);

            var normalizedVin = NormalizeAndValidateVin(vin);

            // e.g. GET /international-vin-decoder?key=...&vin=...
            var request = new RestRequest(options.VinDecoderUrl, Method.Get)
                .AddQueryParameter("vin", normalizedVin);

            return request;
        }

        public static RestRequest CreateRecallsRequest(this CarsXeApiOptions options, string vin)
        {
            ArgumentNullException.ThrowIfNull(options);

            var normalizedVin = NormalizeAndValidateVin(vin);

            // e.g. GET /recalls?key=...&vin=...
            var request = new RestRequest(options.RecallsUrl, Method.Get)
                .AddQueryParameter("vin", normalizedVin);

            return request;
        }

        public static string NormalizeAndValidateVin(string vin)
        {
            if (vin.IsNullOrWhiteSpace())
                throw new UserFriendlyException("VIN is required.");

            var normalized = vin.Trim().ToUpperInvariant();

            if (normalized.Length != 17)
                throw new UserFriendlyException("VIN must be exactly 17 characters.");

            return normalized;
        }
    }
}
