using System.Threading.Tasks;
using WorkShopManagement.External.Vpic;

namespace WorkShopManagement.External.Shared;

public class TestAppService : WorkShopManagementAppService
{
    private readonly IVpicService _vpicService;
    public TestAppService(IVpicService vpicService)
    {
        _vpicService = vpicService;
    }

    public Task<VpicVariableResultDto> DecodeVinExtendedAsync(string vin, string? modelYear = null)
    {
        return _vpicService.DecodeVinExtendedAsync(vin, modelYear);
    }
}
