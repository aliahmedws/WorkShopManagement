using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
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
    public Task<ListResultDto<BayDto>> GetListAsync()
    {
        return _bayAppService.GetListAsync();
    }
}
