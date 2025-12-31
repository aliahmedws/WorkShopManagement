using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using WorkShopManagement.CheckLists;

namespace WorkShopManagement.Controllers.CheckLists;

[RemoteService(IsEnabled = true)]
[ControllerName("CheckLists")]
[Area("app")]
[Route("api/app/check-lists")]
public class CheckListController : WorkShopManagementController, ICheckListAppService
{
    private readonly ICheckListAppService _appService;

    public CheckListController(ICheckListAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<CheckListDto>> GetListAsync(GetCheckListListDto input)
        => _appService.GetListAsync(input);

    [HttpGet("{id}")]
    public Task<CheckListDto> GetAsync(Guid id)
        => _appService.GetAsync(id);

    [HttpPost]
    public Task<CheckListDto> CreateAsync(CreateCheckListDto input)
        => _appService.CreateAsync(input);

    [HttpPut("{id}")]
    public Task<CheckListDto> UpdateAsync(Guid id, UpdateCheckListDto input)
        => _appService.UpdateAsync(id, input);

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
        => _appService.DeleteAsync(id);
}
