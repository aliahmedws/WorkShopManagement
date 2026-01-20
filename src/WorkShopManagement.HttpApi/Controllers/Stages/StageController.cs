using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
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
}
