using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.Stages;

[RemoteService(false)]
[Authorize(WorkShopManagementPermissions.ProductionManager.Default)]
public class StageAppService : WorkShopManagementAppService, IStageAppService
{
    private readonly IStageRepository _stageRepository;
    public StageAppService(IStageRepository stageRepository)
    {
        _stageRepository = stageRepository;
    }

    public async Task<PagedResultDto<StageDto>> GetStageAsync(GetStageInput input)
    {
        if (input?.Stage == null)
        {
            throw new UserFriendlyException("Stage parameter is required.");
        }

        var items = await _stageRepository.GetStageAsync(input.Stage.Value, input.Sorting, input.SkipCount, input.MaxResultCount, input.Filter);

        return new PagedResultDto<StageDto>
        {
            TotalCount = items.TotalCount,
            Items = ObjectMapper.Map<List<StageModel>, List<StageDto>>(items.Items)
        };
    }
}
