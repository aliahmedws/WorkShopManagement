using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using WorkShopManagement.External.CarsXe;

namespace WorkShopManagement.Lookups;

public interface ILookupAppService : IApplicationService
{
    Task<List<GuidLookupDto>> GetCarModelsAsync();
    Task<List<GuidLookupDto>> GetCarOwnersAsync();
    Task<List<GuidLookupDto>> GetCarsAsync();
    Task<List<GuidLookupDto>> GetBaysAsync();
    Task<List<IntLookupDto>> GetPriorityAsync();
    Task<SpecsResponseDto> GetExternalSpecsResponseAsync(string vin);
}
