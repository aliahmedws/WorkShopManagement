using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using WorkShopManagement.External.CarsXE;
using WorkShopManagement.External.Shared;
using WorkShopManagement.Utils.Helpers;

namespace WorkShopManagement.External.CarsXe
{
    public class CarsXeApiOptions
    {
        public const string ConfigurationKey = "External:CarsXe";

        public string BaseUrl { get; set; } = default!;
        public string VinDecoderUrl { get; set; } = default!;
        public string RecallsUrl { get; set; } = default!;
        public string SpecsUrl { get; set; } = default!;
        public string ImagesUrl { get; set; } = default!;
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

            if (options.SpecsUrl.IsNullOrWhiteSpace())
                throw new AbpException($"{CarsXeApiOptions.ConfigurationKey}:SpecsUrl is missing.");

            if (options.ImagesUrl.IsNullOrWhiteSpace())
                throw new AbpException($"{CarsXeApiOptions.ConfigurationKey}:ImagesUrl is missing.");
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

            var normalizedVin = CarHelper.NormalizeAndValidateVin(vin);

            // e.g. GET /international-vin-decoder?key=...&vin=...
            var request = new RestRequest(options.VinDecoderUrl, Method.Get)
                .AddQueryParameter("vin", normalizedVin);

            return request;
        }

        public static RestRequest CreateRecallsRequest(this CarsXeApiOptions options, string vin)
        {
            ArgumentNullException.ThrowIfNull(options);

            var normalizedVin = CarHelper.NormalizeAndValidateVin(vin);

            // e.g. GET /recalls?key=...&vin=...
            var request = new RestRequest(options.RecallsUrl, Method.Get)
                .AddQueryParameter("vin", normalizedVin);

            return request;
        }

        public static RestRequest CreateSpecsRequest(this CarsXeApiOptions options, string vin)
        {
            ArgumentNullException.ThrowIfNull(options);
            var normalizedVin = CarHelper.NormalizeAndValidateVin(vin);
            // e.g. GET /specs?key=...&vin=...
            var request = new RestRequest(options.SpecsUrl, Method.Get)
                .AddQueryParameter("vin", normalizedVin)
                .AddQueryParameter("deepdata", 1);
            return request;
        }

        public static RestRequest CreateImagesRequest(this CarsXeApiOptions options, ImagesSearchRequestDto input)
        {
            ArgumentNullException.ThrowIfNull(options);
            // e.g. GET /images?key=...&make=...&model=...&year=...&trim=...&color=...&angle=...&size=All
            var request = new RestRequest(options.ImagesUrl, Method.Get)
                .AddQueryParameter("make", input.Make)
                .AddQueryParameter("model", input.Model)
                .AddQueryParameter("size", "All");

            if(!string.IsNullOrWhiteSpace(input.Year))  request.AddQueryParameter("year", input.Year!);
            if(!string.IsNullOrWhiteSpace(input.Trim))  request.AddQueryParameter("trim", input.Trim!);
            if(!string.IsNullOrWhiteSpace(input.Color))  request.AddQueryParameter("color", input.Color!);
            if (!string.IsNullOrWhiteSpace(input.Angle)) request.AddQueryParameter("angle", "front");

            if(!string.IsNullOrWhiteSpace(input.Make) && !string.IsNullOrWhiteSpace(input.Model) && !string.IsNullOrWhiteSpace(input.Year) && string.IsNullOrWhiteSpace(input.Trim))
                request.AddQueryParameter("photoType", "exterior");

            return request;
        }

    }
}
