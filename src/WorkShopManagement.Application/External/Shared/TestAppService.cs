using System.Collections.Generic;
using System.Threading.Tasks;
using WorkShopManagement.External.Nhtsa;
using WorkShopManagement.External.Twilio;
using WorkShopManagement.External.Vpic;

namespace WorkShopManagement.External.Shared;

public class TestAppService : WorkShopManagementAppService
{
    private readonly IVpicService _vpicService;
    private readonly INhtsaService _nhtsaService;
    private readonly ITwilioService _twilioService;

    public TestAppService(IVpicService vpicService, INhtsaService nhtsaService, ITwilioService twilioService)
    {
        _vpicService = vpicService;
        _nhtsaService = nhtsaService;
        _twilioService = twilioService;
    }

    public Task<VpicVariableResultDto> DecodeVinExtendedAsync(string vin, string? modelYear = null)
    {
        return _vpicService.DecodeVinExtendedAsync(vin, modelYear);
    }

    public Task<List<NhtsaRecallByVehicleResultDto>> GetRecallByVehicleAsync(string make, string model, string modelYear)
    {
        return _nhtsaService.GetRecallByVehicleAsync(make, model, modelYear);
    }

    public Task<TwilioSmsResponseEto?> SendSmsAsync(string to, string body)
    {
        return _twilioService.SendSmsAsync(to, body);
    }
}
