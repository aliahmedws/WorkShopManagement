using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.Bays;

public interface IBayAppService : IApplicationService
{
    Task<ListResultDto<BayDto>> GetListAsync(GetBayListInput input);
    Task SetIsActiveAsync(Guid id, bool isActive);
}
