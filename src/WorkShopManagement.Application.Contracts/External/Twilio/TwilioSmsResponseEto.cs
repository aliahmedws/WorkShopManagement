using System.Text.Json.Serialization;

namespace WorkShopManagement.External.Twilio;

public class TwilioSmsResponseEto
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("more_info")]
    public string? MoreInfo { get; set; }

    [JsonPropertyName("account_sid")]
    public string? AccountSid { get; set; }

    [JsonPropertyName("api_version")]
    public string? ApiVersion { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    // Twilio returns RFC1123 strings like: "Tue, 06 Jan 2026 16:16:22 +0000"
    [JsonPropertyName("date_created")]
    public string? DateCreated { get; set; }

    [JsonPropertyName("date_sent")]
    public string? DateSent { get; set; }

    [JsonPropertyName("date_updated")]
    public string? DateUpdated { get; set; }

    [JsonPropertyName("direction")]
    public string? Direction { get; set; }

    [JsonPropertyName("error_code")]
    public int? ErrorCode { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("from")]
    public string? From { get; set; }

    [JsonPropertyName("messaging_service_sid")]
    public string? MessagingServiceSid { get; set; }

    // Twilio sends these as strings in the payload shown
    [JsonPropertyName("num_media")]
    public string? NumMedia { get; set; }

    [JsonPropertyName("num_segments")]
    public string? NumSegments { get; set; }

    [JsonPropertyName("price")]
    public string? Price { get; set; }

    [JsonPropertyName("price_unit")]
    public string? PriceUnit { get; set; }

    [JsonPropertyName("sid")]
    public string? Sid { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("subresource_uris")]
    public TwilioSubresourceUrisEto? SubresourceUris { get; set; }

    [JsonPropertyName("to")]
    public string? To { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}

public class TwilioSubresourceUrisEto
{
    [JsonPropertyName("media")]
    public string? Media { get; set; }
}
