using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using Volo.Abp;
using WorkShopManagement.External.Shared;

namespace WorkShopManagement.External.Nhtsa;

public class NhtsaApiOptions
{
    public const string ConfigurationKey = "External:Nhtsa";
    public string BaseUrl { get; set; } = default!;
    public string RecallByVehicleUrl { get; set; } = default!;
}

public class ConfigureNhtsaApiOptions(IConfiguration configuration) : IConfigureOptions<NhtsaApiOptions>
{
    private readonly IConfiguration _configuration = configuration;

    public void Configure(NhtsaApiOptions options)
    {
        _configuration.GetSection(NhtsaApiOptions.ConfigurationKey).Bind(options);
        options.BaseUrl = options.BaseUrl.TrimEnd('/');
        options.RecallByVehicleUrl = options.RecallByVehicleUrl.TrimEnd('/');
    }
}

public static class NhtsaApiOptionsExtensions
{
    public static IRestClient CreateRestClient(this NhtsaApiOptions options, IRestClientFactory restClientFactory)
    {
        return restClientFactory.Create(options.BaseUrl);
    }

    public static RestRequest CreateRecallByVehicleRequest(this NhtsaApiOptions options, string make, string model, string modelYear)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(make);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(modelYear);

        if (string.IsNullOrWhiteSpace(make))
        {
            throw new UserFriendlyException("Make is required.");
        
        }
        if (string.IsNullOrWhiteSpace(model))
        {
            throw new UserFriendlyException("Model is required.");
        }

        if (string.IsNullOrWhiteSpace(modelYear) || !int.TryParse(modelYear, out _))
        {
            throw new UserFriendlyException("Model year must be a valid year (e.g., 2018).");
        }

        var request = new RestRequest(options.RecallByVehicleUrl, Method.Get);
        request.AddQueryParameter("make", Uri.EscapeDataString(make));
        request.AddQueryParameter("model", Uri.EscapeDataString(model));
        request.AddQueryParameter("modelYear", Uri.EscapeDataString(modelYear));

        return request;
    }
}