using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.Stages;

public interface IStageAppService : IApplicationService
{
    Task<PagedResultDto<StageDto>> GetStageAsync(GetStageInput input);
    Task<List<StageBayDto>> GetBaysAsync();
}
