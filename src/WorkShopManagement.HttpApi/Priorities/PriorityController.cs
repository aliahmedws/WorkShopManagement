using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace WorkShopManagement.Priorities;

[RemoteService(IsEnabled = true)]
[ControllerName("Priorities")]
[Area("app")]
[Route("api/app/priorities")]
public class PriorityController : AbpController, IPriorityAppService
{
    private readonly IPriorityAppService _priorityAppService;
    public PriorityController(IPriorityAppService priorityAppService)
    {
        _priorityAppService = priorityAppService;
    }

    [HttpGet("{id}")]
    public async Task<PriorityDto> GetAsync(Guid id)
    {
        return await _priorityAppService.GetAsync(id);
    }

    [HttpGet]
    public async Task<PagedResultDto<PriorityDto>> GetListAsync(GetPriorityListDto input)
    {
        return await _priorityAppService.GetListAsync(input);
    }

    [HttpPost]
    public async Task<PriorityDto> CreateAsync(CreatePriorityDto input)
    {
        return await _priorityAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, UpdatePriorityDto input)
    {
        await _priorityAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _priorityAppService.DeleteAsync(id);
    }
}

