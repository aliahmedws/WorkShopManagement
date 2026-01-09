using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.ListItems;


namespace WorkShopManagement.Controllers.ListItems;

[RemoteService(IsEnabled = true)]
[ControllerName("ListItems")]
[Area("app")]
[Route("api/app/list-items")]
public class ListItemController : WorkShopManagementController, IListItemAppService
{
    private readonly IListItemAppService _appService;

    public ListItemController(IListItemAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<ListItemDto>> GetListAsync(GetListItemListDto input)
        => _appService.GetListAsync(input);

    [HttpGet("{id}")]
    public Task<ListItemDto> GetAsync(Guid id)
        => _appService.GetAsync(id);

    [HttpPost]
    public Task<ListItemDto> CreateAsync(CreateListItemDto input)
        => _appService.CreateAsync(input);

    [HttpPut("{id}")]
    public Task<ListItemDto> UpdateAsync(Guid id, UpdateListItemDto input)
        => _appService.UpdateAsync(id, input);

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
        => _appService.DeleteAsync(id);

    [HttpGet("by-checklist/{checkListId}")]
    public async Task<List<ListItemDto>> GetByCheckListWithDetailsAsync(Guid checkListId)
      => await _appService.GetByCheckListWithDetailsAsync(checkListId);
}
