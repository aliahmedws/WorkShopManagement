using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using WorkShopManagement.External.Shared;

namespace WorkShopManagement.External.Vpic;

public class VpicService : IVpicService, ITransientDependency
{
    private readonly VpicApiOptions _options;
    private readonly IRestClient _client;

    public VpicService(IOptions<VpicApiOptions> options, IRestClientFactory clientFactory)
    {
        _options = options.Value;
        _client = _options.CreateRestClient(clientFactory);
    }
    
    public async Task<VpicVariableResultDto> DecodeVinExtendedAsync(string vin, string? modelYear = null)
    {
        vin = (vin ?? "").Trim().ToUpperInvariant();

        var request = _options.CreateDecodeVinExtendedRequest(vin, modelYear);

        var response = await _client.ExecuteGetAsync<VpicDecodeVinResponseEto>(request);

        VpicHelper.EnsureSuccessResponse(response);

        return VpicHelper.ParseResult(vin, response.Data?.Results);
    }
}
