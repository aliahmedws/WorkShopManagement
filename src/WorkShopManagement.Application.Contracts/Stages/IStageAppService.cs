using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace WorkShopManagement.Stages;

public interface IStageAppService : IApplicationService
{
    Task SetUseProductionClassicViewAsync(bool useClassicView);
    Task<bool> GetUseProductionClassicViewAsync();
    Task<ListResultDto<StageDto>> GetAllAsync(string? filter = null);
    Task<PagedResultDto<StageDto>> GetStageAsync(GetStageInput input);
    Task<List<StageBayDto>> GetBaysAsync();
    Task<IRemoteStreamContent> GetListAsExcelAsync();
}
