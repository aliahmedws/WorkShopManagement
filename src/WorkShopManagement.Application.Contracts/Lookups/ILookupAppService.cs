using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.Lookups;

public interface ILookupAppService : IApplicationService
{
    Task<List<GuidLookupDto>> GetCarModelsAsync();
    Task<List<GuidLookupDto>> GetCarOwnersAsync();
}
