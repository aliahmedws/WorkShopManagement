using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;


namespace WorkShopManagement.ListItems;

[RemoteService(IsEnabled = true)]
[ControllerName("ListItems")]
[Area("app")]
[Route("api/app/list-items")]
public class ListItemController : AbpController, IListItemAppService
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
}
