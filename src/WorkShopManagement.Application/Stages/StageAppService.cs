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

    public async Task<ListResultDto<StageDto>> GetAllAsync(string? filter = null)
    {
        var items = await _stageRepository.GetStageAsync(filter: filter, maxResultCount: int.MaxValue);

        return new ListResultDto<StageDto>
        {
            Items = ObjectMapper.Map<List<StageModel>, List<StageDto>>(items.Items)
        };
    }

    public async Task<PagedResultDto<StageDto>> GetStageAsync(GetStageInput input)
    {
        if (input?.Stage == null)
        {
            throw new UserFriendlyException("Stage parameter is required.");
        }

        var items = await _stageRepository.GetStageAsync(input.Stage, input.Sorting, input.SkipCount, input.MaxResultCount, input.Filter);

        return new PagedResultDto<StageDto>
        {
            TotalCount = items.TotalCount,
            Items = ObjectMapper.Map<List<StageModel>, List<StageDto>>(items.Items)
        };
    }

    public async Task<List<StageBayDto>> GetBaysAsync()
    {
        var bays = await _stageRepository.GetBaysAsync();
        return ObjectMapper.Map<List<StageBayModel>, List<StageBayDto>>(bays);
    }
}
