using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.Lookups;

public interface ILookupRepository : IRepository
{
    Task<List<GuidLookup>> GetCarModelsAsync();
    Task<List<GuidLookup>> GetCarOwnersAsync();
    Task<List<GuidLookup>> GetBaysAsync();
    Task<List<IntLookup>> GetPrioritiesAsync();
}
