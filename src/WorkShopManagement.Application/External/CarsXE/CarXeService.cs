using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;
using WorkShopManagement.VinInfos;
using WorkShopManagement.External.Shared;


namespace WorkShopManagement.External.CarsXE
{
    //[RemoteService(false)]
    public class CarXeService : ApplicationService, ICarXeService
    {
        private readonly IRepository<VinInfo, Guid> _vinInfoRepository;
        private readonly VinInfoManager _vinInfoManager;
        private readonly IRestClientFactory _restClientFactory;
        private readonly CarsXeApiOptions _carsXeOptions;
        private readonly IClock _clock;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CarXeService(
            IRepository<VinInfo, Guid> vinInfoRepository,
            VinInfoManager vinInfoManager,
            IRestClientFactory restClientFactory,
            IOptions<CarsXeApiOptions> carsXeOptions,
            IClock clock)
        {
            _vinInfoRepository = vinInfoRepository;
            _vinInfoManager = vinInfoManager;
            _restClientFactory = restClientFactory;
            _carsXeOptions = carsXeOptions.Value;
            _clock = clock;
        }

        public async Task<VinResponseDto> GetVinAsync(
            string vinNo,
            CancellationToken ct = default)
        {
            var vin = CarsXeApiOptionsExtensions.NormalizeAndValidateVin(vinNo);

            var existing = await _vinInfoRepository.FirstOrDefaultAsync(x => x.VinNo == vin, cancellationToken: ct);

            // If cached JSON exists, deserialize and return
            if (existing?.VinResponse != null && !string.IsNullOrWhiteSpace(existing.VinResponse))
            {
                return DeserializeOrThrow<VinResponseDto>(existing.VinResponse);
            }

            // Call CarsXE
            var client = _carsXeOptions.CreateRestClient(_restClientFactory);
            var request = _carsXeOptions.CreateVinDecoderRequest(vin);

            var json = await ExecuteForJsonAsync(client, request, ct);

            // Save raw JSON
            await _vinInfoManager.UpsertVinAsync(vin, json, _clock.Now, ct);

            return DeserializeOrThrow<VinResponseDto>(json);
        }

        public async Task<RecallsResponseDto> GetRecallAsync(
            string vinNo,
            CancellationToken ct = default)
        {
            var vin = CarsXeApiOptionsExtensions.NormalizeAndValidateVin(vinNo);

            var existing = await _vinInfoRepository.FirstOrDefaultAsync(x => x.VinNo == vin, cancellationToken: ct);

            // If cached JSON exists, deserialize and return
            if (existing?.RecallResponse != null && !string.IsNullOrWhiteSpace(existing.RecallResponse))
            {
                return DeserializeOrThrow<RecallsResponseDto>(existing.RecallResponse);
            }

            // Call CarsXE
            var client = _carsXeOptions.CreateRestClient(_restClientFactory);
            var request = _carsXeOptions.CreateRecallsRequest(vin);

            var json = await ExecuteForJsonAsync(client, request, ct);

            // Save raw JSON
            await _vinInfoManager.UpsertRecallAsync(vin, json, _clock.Now, ct);

            return DeserializeOrThrow<RecallsResponseDto>(json);
        }

        private static async Task<string> ExecuteForJsonAsync(
            IRestClient client,
            RestRequest request,
            CancellationToken ct)
        {
            var response = await client.ExecuteAsync(request, ct);

            // CarsXE docs specify success flag in JSON; but also check HTTP success.
            if (!response.IsSuccessful)
            {
                throw new UserFriendlyException(
                    message: $"CarsXE request failed ({(int)response.StatusCode}).",
                    details: response.Content
                );
            }

            return response.Content ?? "{}";
        }

        private static T DeserializeOrThrow<T>(string json)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<T>(json, JsonOptions);
                if (dto == null)
                    throw new UserFriendlyException("External API returned an empty response.");

                return dto;
            }
            catch (JsonException ex)
            {
                throw new UserFriendlyException("Failed to parse external API response JSON.", ex.Message);
            }
        }
    }
}
