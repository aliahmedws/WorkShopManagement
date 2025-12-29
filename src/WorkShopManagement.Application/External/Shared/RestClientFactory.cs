using RestSharp;
using System;
using Volo.Abp.DependencyInjection;

namespace WorkShopManagement.External.Shared;

public sealed class RestClientFactory : IRestClientFactory, ITransientDependency
{
    public RestClient Create(string baseUrl, RestClientProfile? profile = null)
    {
        profile ??= new();
        var timeout = profile.Timeout <= TimeSpan.Zero ? TimeSpan.FromSeconds(30) : profile.Timeout;

        var options = new RestClientOptions(baseUrl.TrimEnd('/'))
        {
            Timeout = timeout,
            ThrowOnAnyError = false
        };

        var client = new RestClient(options);

        foreach (var param in profile.DefaultQueryParams)
        {
            if (!string.IsNullOrWhiteSpace(param.Key))
            {
                client.AddDefaultQueryParameter(param.Key, param.Value);
            }
        }

        foreach (var header in profile.DefaultHeaders)
        {
            if (!string.IsNullOrWhiteSpace(header.Key))
            {
                client.AddDefaultHeader(header.Key, header.Value);
            }
        }

        return client;
    }
}
