using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using WorkShopManagement.External.Shared;

namespace WorkShopManagement.External.Nhtsa;

public class NhtsaService : INhtsaService, ITransientDependency
{
    private readonly NhtsaApiOptions _options;
    private readonly IRestClient _client;
    private readonly IObjectMapper _mapper;

    public NhtsaService(IOptions<NhtsaApiOptions> options, IRestClientFactory clientFactory, IObjectMapper mapper)
    {
        _options = options.Value;
        _client = _options.CreateRestClient(clientFactory);
        _mapper = mapper;
    }

    public async Task<List<NhtsaRecallByVehicleResultDto>> GetRecallByVehicleAsync(string make, string model, string modelYear)
    {
        make = (make ?? "").Trim();
        model = (model ?? "").Trim();
        modelYear = (modelYear ?? "").Trim();

        var request = _options.CreateRecallByVehicleRequest(make, model, modelYear);

        var response = await _client.ExecuteGetAsync<NhtsaRecallByVehicleResponseEto>(request);

        EnsureSuccessResponse(response);

        var results = response.Data?.Results ?? [];

        var mapped = _mapper.Map<List<NhtsaRecallByVehicleResultEto>, List<NhtsaRecallByVehicleResultDto>>(results);

        return mapped;
    }

    private static void EnsureSuccessResponse(RestResponse<NhtsaRecallByVehicleResponseEto> response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = "NHTSA Recall by vehicle request failed with Status Code: {0}, Message: {1}";
            var parsedError = string.Format(errorMessage, (int)response.StatusCode, response.ErrorMessage ?? response.ErrorException?.Message);
            throw new UserFriendlyException(parsedError);
        }
    }
}
