using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.Issues;

namespace WorkShopManagement.Controllers.Issues;

[RemoteService]
[ControllerName("Issue")]
[Area("app")]
[Route("api/app/issues")]
public class IssueController(IIssueAppService service) : WorkShopManagementController, IIssueAppService
{
    [HttpGet("{carId}")]
    public Task<ListResultDto<IssueDto>> GetListByCarAsync(Guid carId)
    {
        return service.GetListByCarAsync(carId);
    }

    [HttpPost]
    public Task UpsertAsync(Guid carId, UpsertIssuesRequestDto input)
    {
        return service.UpsertAsync(carId, input);
    }
}
