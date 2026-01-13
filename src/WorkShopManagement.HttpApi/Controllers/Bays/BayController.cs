using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.Bays;

namespace WorkShopManagement.Controllers.Bays;

[RemoteService(IsEnabled = true)]
[ControllerName("Bays")]
[Area("app")]
[Route("api/app/bays")]
public class BayController : WorkShopManagementController, IBayAppService
{
    private readonly IBayAppService _bayAppService;

    public BayController(IBayAppService bayAppService)
    {
        _bayAppService = bayAppService;
    }

    [HttpGet]
    public Task<ListResultDto<BayDto>> GetListAsync(GetBayListInput input)
    {
        return _bayAppService.GetListAsync(input);
    }

    [HttpPost("{id}/active/{isActive}")]
    public Task SetIsActiveAsync(Guid id, bool isActive)
    {
        return _bayAppService.SetIsActiveAsync(id, isActive);
    }
}
