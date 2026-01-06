using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using WorkShopManagement.External.Shared;

namespace WorkShopManagement.External.Twilio;

public class TwilioService : ITwilioService, ITransientDependency
{
    private readonly TwilioApiOptions _options;
    private readonly IRestClientFactory _clientFactory;
    private readonly ILogger<TwilioService> _logger;

    public TwilioService(IOptions<TwilioApiOptions> options, IRestClientFactory clientFactory, ILogger<TwilioService> logger)
    {
        _options = options.Value;
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<TwilioSmsResponseEto?> SendSmsAsync(string to, string body)
    {
        to = (to ?? "").Trim().EnsureStartsWith('+');
        body = (body ?? "").Trim();

        var client = _options.CreateSmsClient(_clientFactory);
        var request = _options.CreateSmsRequest(to, body);

        var response = await client.ExecutePostAsync<TwilioSmsResponseEto>(request);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new UserFriendlyException("Account has reached it's maximum number of SMS");
            }

            _logger.LogError("Error in Twilio SMS API request with StatusCode: {statusCode}, Content: {content}", response.StatusCode, response.Content);
            if (response.ErrorException != null)
            {
                _logger.LogException(response.ErrorException);
            }

            throw new UserFriendlyException("Twilio SMS request failed");
        }
        return response.Data;
    }
}