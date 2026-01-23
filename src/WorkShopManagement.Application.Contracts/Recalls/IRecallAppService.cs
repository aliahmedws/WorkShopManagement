using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.Recalls
{
    public interface IRecallAppService : IApplicationService
    {
        Task<List<RecallDto>> GetListByCarAsync(Guid carId);
        Task<List<RecallDto>> AddOrUpdateRecallsAsync(Guid carId, List<RecallDto> inputs);
        Task<List<RecallDto>> GetExternalRecallsAsync(Guid carId);


        Task<RecallDto> GetAsync(Guid id);
        Task<RecallDto> CreateAsync(CreateRecallDto input);
        Task<RecallDto> UpdateAsync(Guid id, UpdateRecallDto input);    
        Task DeleteAsync(Guid id);

    }
}
