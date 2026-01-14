using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.External.CarsXe;
using WorkShopManagement.Utils.Helpers;
using WorkShopManagement.VinInfos;

namespace WorkShopManagement.Lookups;

[RemoteService(false)]
[Authorize]
public class LookupAppService(ILookupRepository lookupRepository, CarXeService carXeService) : WorkShopManagementAppService, ILookupAppService
{
    private readonly ILookupRepository _lookupRepository = lookupRepository;
    private readonly ICarXeService _carXeService = carXeService;

    public async Task<List<GuidLookupDto>> GetCarModelsAsync()
    {
        var carModels = await _lookupRepository.GetCarModelsAsync();
        return Map(carModels);
    }

    public async Task<List<GuidLookupDto>> GetCarOwnersAsync()
    {
        var owners = await _lookupRepository.GetCarOwnersAsync();
        return Map(owners);
    }

    public async Task<List<GuidLookupDto>> GetCarsAsync()
    {
        var cars = await _lookupRepository.GetCarsAsync();
        return Map(cars);
    }

    public async Task<List<GuidLookupDto>> GetBaysAsync()
    {
        var bays = await _lookupRepository.GetBaysAsync();
        return Map(bays);
    }

    List<GuidLookupDto> Map(List<GuidLookup> input) => ObjectMapper.Map<List<GuidLookup>, List<GuidLookupDto>>(input);

    public async Task<List<IntLookupDto>> GetPriorityAsync()
    {
       var Priorities = await _lookupRepository.GetPrioritiesAsync();
        return ObjectMapper.Map<List<IntLookup>, List<IntLookupDto>>(Priorities);
    }

    public async Task<SpecsResponseDto> GetExternalSpecsResponseAsync(string vin)
    {
        var vinNo = CarHelper.NormalizeAndValidateVin(vin);
        return await _carXeService.GetSpecsAsync(vinNo);
    }
}
