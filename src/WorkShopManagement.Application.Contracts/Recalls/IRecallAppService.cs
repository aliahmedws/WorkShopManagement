using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.Recalls
{
    public interface IRecallAppService : IApplicationService
    {
        Task<RecallDto> GetAsync(Guid id);
        Task<List<RecallDto>> GetListByCarAsync(Guid carId);
        Task<RecallDto> CreateAsync(CreateRecallDto input);
        Task<RecallDto> UpdateAsync(Guid id, UpdateRecallDto input);    
        Task DeleteAsync(Guid id);
        Task<List<ExternalRecallDetailDto>> GetRecallsFromExternalServiceAsync(Guid carId);

    }
}
