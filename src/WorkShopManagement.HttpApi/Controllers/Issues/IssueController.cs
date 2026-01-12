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

    [HttpPost("{carId}")]
    public Task<IssueDto> UpsertAsync(Guid carId, UpsertIssueDto input)
    {
        return service.UpsertAsync(carId, input);
    }

    [HttpGet]
    public Task<PagedResultDto<IssueListDto>> GetListAsync(GetIssueListInput input)
    {
        return service.GetListAsync(input);
    }
}
