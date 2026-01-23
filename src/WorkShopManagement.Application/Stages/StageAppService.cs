using Microsoft.AspNetCore.Authorization;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using Volo.Abp.SettingManagement;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Permissions;
using WorkShopManagement.Settings;

namespace WorkShopManagement.Stages;

[RemoteService(false)]
[Authorize(WorkShopManagementPermissions.ProductionManager.Default)]
public class StageAppService : WorkShopManagementAppService, IStageAppService
{
    private readonly IStageRepository _stageRepository;
    private readonly ISettingManager _settingManager;

    public StageAppService(IStageRepository stageRepository, ISettingManager settingManager)
    {
        _stageRepository = stageRepository;
        _settingManager = settingManager;
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

    public async Task SetUseProductionClassicViewAsync(bool useClassicView)
    {
        await _settingManager.SetForCurrentUserAsync(WorkShopManagementSettings.UseProductionClassicView, useClassicView.ToString());
    }

    public async Task<bool> GetUseProductionClassicViewAsync()
    {
        var useClassicView = await _settingManager.GetOrNullForCurrentUserAsync(WorkShopManagementSettings.UseProductionClassicView);
        return useClassicView != null && bool.Parse(useClassicView);
    }

    public async Task<IRemoteStreamContent> GetListAsExcelAsync()
    {
        var result = await GetAllAsync();

        var itemsByStage = (result.Items ?? [])
            .GroupBy(x => x.Stage)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());

        return await StageExcelHelper.GenerateAsync(itemsByStage, L);
    }
}
