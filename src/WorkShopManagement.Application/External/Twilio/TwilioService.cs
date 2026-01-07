using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using WorkShopManagement.External.Shared;

namespace WorkShopManagement.External.Twilio;

public class TwilioService : ITwilioService, ITransientDependency
{
    private readonly TwilioApiOptions _options;
    private readonly IRestClientFactory _clientFactory;

#pragma warning disable CA1873 // Avoid potentially expensive logging
    private readonly ILogger<TwilioService> _logger;

    public TwilioService(IOptions<TwilioApiOptions> options, IRestClientFactory clientFactory, ILogger<TwilioService> logger)
    {
        _options = options.Value;
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<TwilioSmsResponseEto?> SendSmsAsync(string to, string body)
    {
        var methodName = nameof(SendSmsAsync);
        to = (to ?? "").Trim().EnsureStartsWith('+');
        body = (body ?? "").Trim();

        _logger.LogInformation("{methodName}: Sending SMS to: {to}", methodName, to);

        var client = _options.CreateSmsClient(_clientFactory);
        var request = _options.CreateSmsRequest(to, body);

        var response = await client.ExecutePostAsync<TwilioSmsResponseEto>(request);
        _logger.LogInformation("{methodName}: Twilio SMS API executed", methodName);

        var redactedResponse = BuildRedactedTwilioJsonForLog(response.Content);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new UserFriendlyException("Account has reached it's maximum number of SMS");
            }

            _logger.LogError("Error in Twilio SMS API request with StatusCode: {statusCode}, Content: {content}", response.StatusCode, redactedResponse);
            if (response.ErrorException != null)
            {
                _logger.LogException(response.ErrorException);
            }

            throw new UserFriendlyException("Twilio SMS request failed");
        }

        _logger.LogInformation("{methodName}: Received following response from Twilio SMS API: {response}", methodName, redactedResponse);
        return response.Data;
    }

    private static string BuildRedactedTwilioJsonForLog(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        try
        {
            var node = JsonNode.Parse(content);

            if (node is JsonObject obj)
            {
                if (obj.ContainsKey("body"))
                    obj["body"] = "***REDACTED***";

                if (obj.ContainsKey("account_sid")) obj["account_sid"] = "***REDACTED***";
            }

            return node?.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = true
            }) ?? string.Empty;
        }
        catch
        {
            return content.Length <= 2000 ? content : content[..2000] + "...(truncated)";
        }
    }
}