using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;
using System;
using Volo.Abp;
using WorkShopManagement.External.Nhtsa;
using WorkShopManagement.External.Shared;

namespace WorkShopManagement.External.Twilio;

public class TwilioApiOptions
{
    public const string ConfigurationKey = "External:Twilio";

    public string AccountSid { get; set; } = default!;
    public string AuthToken { get; set; } = default!;
    public string MessageUrl { get; set; } = default!;
    public string MessagingServiceSid { get; set; } = default!;
}

public class ConfigureTwilioApiOptions(IConfiguration configuration) : IConfigureOptions<TwilioApiOptions>
{
    private readonly IConfiguration _configuration = configuration;

    public void Configure(TwilioApiOptions options)
    {
        _configuration.GetSection(TwilioApiOptions.ConfigurationKey).Bind(options);
        options.MessageUrl = string.Format(options.MessageUrl.TrimEnd('/'), options.AccountSid);
    }
}

public static class TwilioApiOptionsExtensions
{
    public static IRestClient CreateSmsClient(this TwilioApiOptions options, IRestClientFactory restClientFactory)
    {
        return CreateRestClient(options, restClientFactory, options.MessageUrl);
    }

    private static RestClient CreateRestClient(this TwilioApiOptions options, IRestClientFactory restClientFactory, string baseUrl)
    {
        var profile = new RestClientProfile
        {
            Authenticator = new HttpBasicAuthenticator(options.AccountSid, options.AuthToken)
        };

        return restClientFactory.Create(baseUrl, profile);
    }

    public static RestRequest CreateSmsRequest(this TwilioApiOptions options, string to, string body)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(to))
        {
            throw new UserFriendlyException("To is required.");

        }
        if (string.IsNullOrWhiteSpace(body))
        {
            throw new UserFriendlyException("Body is required.");
        }
        if (!to.StartsWith('+'))
        {
            throw new UserFriendlyException("To must start with +");
        }

        var request = new RestRequest("", Method.Post);
        request.AddOrUpdateParameter("MessagingServiceSid", options.MessagingServiceSid, ParameterType.GetOrPost);
        request.AddOrUpdateParameter("To", to, ParameterType.GetOrPost);
        request.AddOrUpdateParameter("Body", body, ParameterType.GetOrPost);

        return request;
    }
}