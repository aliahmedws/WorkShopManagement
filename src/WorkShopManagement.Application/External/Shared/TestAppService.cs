using System.Collections.Generic;
using System.Threading.Tasks;
using WorkShopManagement.External.Nhtsa;
using WorkShopManagement.External.Vpic;

namespace WorkShopManagement.External.Shared;

public class TestAppService : WorkShopManagementAppService
{
    private readonly IVpicService _vpicService;
    private readonly INhtsaService _nhtsaService;

    public TestAppService(IVpicService vpicService, INhtsaService nhtsaService)
    {
        _vpicService = vpicService;
        _nhtsaService = nhtsaService;
    }

    public Task<VpicVariableResultDto> DecodeVinExtendedAsync(string vin, string? modelYear = null)
    {
        return _vpicService.DecodeVinExtendedAsync(vin, modelYear);
    }

    public Task<List<NhtsaRecallByVehicleResultDto>> GetRecallByVehicleAsync(string make, string model, string modelYear)
    {
        return _nhtsaService.GetRecallByVehicleAsync(make, model, modelYear);
    }
}
