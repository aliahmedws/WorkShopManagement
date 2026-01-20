using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using WorkShopManagement.Stages;

namespace WorkShopManagement.Controllers.Stages;

[RemoteService]
[ControllerName("Stage")]
[Area("app")]
[Route("api/app/stages")]
public class StageController(IStageAppService service) : WorkShopManagementController, IStageAppService
{
    [HttpGet("all-stages")]
    public Task<ListResultDto<StageDto>> GetAllAsync(string? filter = null)
    {
        return service.GetAllAsync(filter);
    }

    [HttpGet("stage")]
    public Task<PagedResultDto<StageDto>> GetStageAsync(GetStageInput input)
    {
        return service.GetStageAsync(input);
    }

    [HttpGet("bays")]
    public Task<List<StageBayDto>> GetBaysAsync()
    {
        return service.GetBaysAsync();
    }

    [HttpPost("production-classic-view/{useClassicView}")]
    public Task SetUseProductionClassicViewAsync(bool useClassicView)
    {
        return service.SetUseProductionClassicViewAsync(useClassicView);
    }

    [HttpGet("production-classic-view")]
    public Task<bool> GetUseProductionClassicViewAsync()
    {
        return service.GetUseProductionClassicViewAsync();
    }

    [HttpPost("excel")]
    public Task<IRemoteStreamContent> GetListAsExcelAsync()
    {
        return service.GetListAsExcelAsync();
    }
}
