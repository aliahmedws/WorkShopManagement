using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

namespace WorkShopManagement.External.Vpic;

internal class VpicHelper
{
    internal static VpicVariableResultDto ParseResult(string vin, List<VpicVariableResultEto>? variables)
    {
        string? GetValue(string variable) =>
                (variables ?? [])
                    .FirstOrDefault(r =>
                        string.Equals(r.Variable, variable, StringComparison.OrdinalIgnoreCase) &&
                        !string.IsNullOrWhiteSpace(r.Value))
                    ?.Value;

        var suggestedVin = GetValue("Suggested VIN");
        var vinMatched = suggestedVin.IsNullOrWhiteSpace() || suggestedVin.Equals(vin, StringComparison.OrdinalIgnoreCase);

        var errorCode = GetValue("Error Code")?.Trim();
        var noError = string.Equals(errorCode, "0", StringComparison.Ordinal);

        if (noError && vinMatched)
        {
            return new VpicVariableResultDto
            {
                Model = GetValue("Model"),
                ModelYear = GetValue("Model Year")
            };
        }

        var errorText = GetValue("Error Text");

        return new VpicVariableResultDto
        {
            SuggestedVin = suggestedVin,
            Error = !string.IsNullOrWhiteSpace(errorText)
                    ? errorText
                    : $"VIN decode failed (Error Code: {errorCode ?? "unknown"})."
        };
    }

    internal static void EnsureSuccessResponse(RestResponse<VpicDecodeVinResponseEto> response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = "NHTSA vPIC VIN decode request failed with Status Code: {0}, Message: {1}";
            var parsedError = string.Format(errorMessage, (int)response.StatusCode, response.ErrorMessage ?? response.ErrorException?.Message);
            throw new UserFriendlyException(parsedError);
        }

        if (response.Data?.Results is null)
        {
            throw new UserFriendlyException("NHTSA vPIC returned an unexpected response (missing Results).");
        }
    }
}