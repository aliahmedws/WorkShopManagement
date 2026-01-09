using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkShopManagement.Lookups;

namespace WorkShopManagement.Controllers.Lookups;

[ControllerName("Lookup")]
[Area("app")]
[Route("api/app/lookups")]
public class LookupController(ILookupAppService lookupAppService) : WorkShopManagementController, ILookupAppService
{
    [HttpGet("bays")]
    public Task<List<GuidLookupDto>> GetBaysAsync()
    {
        return lookupAppService.GetBaysAsync();
    }

    [HttpGet("car-models")]
    public Task<List<GuidLookupDto>> GetCarModelsAsync()
    {
        return lookupAppService.GetCarModelsAsync();
    }

    [HttpGet("car-owners")]
    public Task<List<GuidLookupDto>> GetCarOwnersAsync()
    {
        return lookupAppService.GetCarOwnersAsync();
    }

    [HttpGet("cars")]
    public Task<List<GuidLookupDto>> GetCarsAsync()
    {
        return lookupAppService.GetCarsAsync();
    }

    [HttpGet("priorities")]
    public Task<List<IntLookupDto>> GetPriorityAsync()
    {
        return lookupAppService.GetPriorityAsync();
    }
}
