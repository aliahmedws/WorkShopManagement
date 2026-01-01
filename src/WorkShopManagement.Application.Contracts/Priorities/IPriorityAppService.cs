using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.Priorities;

public interface IPriorityAppService : IApplicationService
{
    Task<PriorityDto> GetAsync(Guid id);
    Task<PagedResultDto<PriorityDto>> GetListAsync(GetPriorityListDto input);
    Task<PriorityDto> CreateAsync(CreatePriorityDto input);
    Task UpdateAsync(Guid id, UpdatePriorityDto input);
    Task DeleteAsync(Guid id);
}

