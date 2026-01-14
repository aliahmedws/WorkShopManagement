using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using WorkShopManagement.External.CarsXE;
using WorkShopManagement.External.Shared;
using WorkShopManagement.Utils.Helpers;
using WorkShopManagement.VinInfos;


namespace WorkShopManagement.External.CarsXe
{
    //[RemoteService(false)]
    public class CarXeService : ITransientDependency, ICarXeService
    {
        private readonly VinInfoManager _vinInfoManager;
        private readonly IRestClientFactory _restClientFactory;
        private readonly CarsXeApiOptions _carsXeOptions;
        private readonly IClock _clock;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CarXeService(
            VinInfoManager vinInfoManager,
            IRestClientFactory restClientFactory,
            IOptions<CarsXeApiOptions> carsXeOptions,
            IClock clock)
        {
            _vinInfoManager = vinInfoManager;
            _restClientFactory = restClientFactory;
            _carsXeOptions = carsXeOptions.Value;
            _clock = clock;
        }

        public async Task<VinResponseDto> GetVinAsync(string vinNo, CancellationToken ct = default)
        {
            var vin = CarHelper.NormalizeAndValidateVin(vinNo);

            var existing = await _vinInfoManager.FindVinAsync(vin, ct);

            // If cached JSON exists, deserialize and return
            if (!string.IsNullOrWhiteSpace(existing?.VinResponse))
            {
                return DeserializeOrThrow<VinResponseDto>(existing.VinResponse);
            }

            // Call CarsXE
            var client = _carsXeOptions.CreateRestClient(_restClientFactory);
            var request = _carsXeOptions.CreateVinDecoderRequest(vin);

            var response = await client.ExecutePostAsync<VinResponseDto>(request, ct);

            var data = response.Data ?? null;

            if(response.IsSuccessful && data != null)
            {

                if (!data.Success)
                {
                    if (string.IsNullOrWhiteSpace(data.Message))
                    {
                        data.Message = "Failed to retrieve VIN information from CarXe API.";
                    }
                    return data;
                }

                if (data.Attributes == null)
                {
                    data.Success = false;
                    data.Message = "No data found for the provided VIN.";
                    return data;
                }

                var json = JsonSerializer.Serialize(data, JsonOptions);
                await _vinInfoManager.UpsertVinAsync(vin, json, _clock.Now, ct);

                return data!;
                

            }

            return new VinResponseDto
            {
                Success = false,
                Message = response.ErrorMessage ?? "External API Failed.",
            };

        }

        public async Task<RecallsResponseDto> GetRecallAsync(
            string vinNo,
            CancellationToken ct = default)
        {
            var vin = CarHelper.NormalizeAndValidateVin(vinNo);

            var existing = await _vinInfoManager.FindVinAsync(vin, ct);

            // If cached JSON exists, deserialize and return
            if (!string.IsNullOrWhiteSpace(existing?.RecallResponse))
            {
                return DeserializeOrThrow<RecallsResponseDto>(existing.RecallResponse);
            }

            // Call CarsXE
            var client = _carsXeOptions.CreateRestClient(_restClientFactory);
            var request = _carsXeOptions.CreateRecallsRequest(vin);
            var response = await client.ExecutePostAsync<RecallsResponseDto>(request, ct);

            var data = response.Data;
            if (response.IsSuccessful && data != null)
            {
                if (!data.Success)
                {
                    if (string.IsNullOrWhiteSpace(data.Message))
                    {
                        data.Message = "Failed to retrieve recall information from CarXe API.";
                    }
                    return data;
                }


                var json = JsonSerializer.Serialize(data, JsonOptions);

                await _vinInfoManager.UpsertRecallAsync(vin, json, _clock.Now, ct);

                return data;
                
            }

            return new RecallsResponseDto
            {
                Success = false,
                Message = response.ErrorMessage ?? "CarXe API Failed.",
            };

        }



        public async Task<SpecsResponseDto> GetSpecsAsync(
            string vinNo,
            CancellationToken ct = default)
        {

            var vin = CarHelper.NormalizeAndValidateVin(vinNo);

            var existing = await _vinInfoManager.FindVinAsync(vin, ct);

            if (!string.IsNullOrWhiteSpace(existing?.SpecsResponse))
            {
                return DeserializeOrThrow<SpecsResponseDto>(existing.SpecsResponse);
            }

            // Call CarsXE
            var client = _carsXeOptions.CreateRestClient(_restClientFactory);
            var request = _carsXeOptions.CreateSpecsRequest(vin);
            var response = await client.ExecutePostAsync<SpecsResponseDto>(request, ct);
            var data = response.Data;
            if (response.IsSuccessful && data != null)
            {
                if (!data.Success)
                {
                    if (string.IsNullOrWhiteSpace(data.Message))
                    {
                        data.Message = "Failed to retrieve VIN information and specifications from CarXe API.";
                    }
                    return data;
                }

                if(data.Attributes == null)
                {
                    data.Success = false;
                    data.Message = "No data found for the provided VIN.";
                    return data;
                }

                // Can Remove if only doing Filtering on Frontend. For Case [""] 
                //data.Attributes.ExteriorColor?
                //    .Select(x => x.Trim())
                //    .Where(x => x.Length > 0)
                //    .ToList();


                var json = JsonSerializer.Serialize(data, JsonOptions);
                await _vinInfoManager.UpsertSpecsAsync(vin, json, _clock.Now, ct);
                return data;
            }
            return new SpecsResponseDto
            {
                Success = false,
                Message = response.ErrorMessage ?? "Failed to retrieve VIN information and specifications from CarXe API.",
            };
        }

        public async Task<ImagesResponseDto> GetImagesAsync(
            ImagesSearchRequestDto requestDto,
            CancellationToken ct = default)
        {

            var vin = CarHelper.NormalizeAndValidateVin(requestDto.Vin);
            var existing = await _vinInfoManager.FindVinAsync(vin, ct);
            if (!string.IsNullOrWhiteSpace(existing?.ImagesResponse))
            {
                return DeserializeOrThrow<ImagesResponseDto>(existing.ImagesResponse);
            }

            // Call CarsXE
            var client = _carsXeOptions.CreateRestClient(_restClientFactory);
            var request = _carsXeOptions.CreateImagesRequest(requestDto);
            var response = await client.ExecutePostAsync<ImagesResponseDto>(request, ct);
            var data = response.Data ?? null;
            if (response.IsSuccessful && data != null)
            {
                if (!data.Success)
                {
                    if (string.IsNullOrWhiteSpace(data.Message))
                    {
                        data.Message = "Failed to retrieve vehicle images from CarXe API.";
                    }
                    return data;
                }

                if (data.Images == null || data.Images.Count == 0)
                {
                    data.Success = false;
                    data.Message = "No images found for the provided VIN.";
                    return data;
                }

                var json = JsonSerializer.Serialize(data, JsonOptions);
                await _vinInfoManager.UpsertImagesAsync(vin, json, _clock.Now, ct);
                return data;
            }

            return new ImagesResponseDto
            {
                Success = false,
                Message = response.ErrorMessage ?? "External API Failed.",
            };
        }

        private static T DeserializeOrThrow<T>(string json)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<T>(json, JsonOptions);
                if (dto == null)
                    throw new UserFriendlyException("Failed to parse external API response JSON.");

                return dto;
            }
            catch (JsonException ex)
            {
                throw new UserFriendlyException("Failed to parse external API response JSON.", ex.Message);
            }
        }
    }
}
