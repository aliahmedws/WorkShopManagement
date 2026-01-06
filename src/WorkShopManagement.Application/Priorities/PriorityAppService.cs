using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.Priorities;

[RemoteService(isEnabled: false)]
[Authorize(WorkShopManagementPermissions.Priorities.Default)]
public class PriorityAppService : ApplicationService, IPriorityAppService
{
    private readonly IPriorityRepository _priorityRepository;
    private readonly PriorityManager _priorityManager;

    public PriorityAppService(IPriorityRepository priorityRepository, PriorityManager priorityManager)
    {
        _priorityRepository = priorityRepository;
        _priorityManager = priorityManager;
    }
    public async Task<PriorityDto> GetAsync(Guid id)
    {
        var priority = await _priorityRepository.GetAsync(id);
        return ObjectMapper.Map<Priority, PriorityDto>(priority);
    }
    public async Task<PagedResultDto<PriorityDto>> GetListAsync(GetPriorityListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(Priority.Number);
        }
        var priorities = await _priorityRepository.GetListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            input.Filter
        );
        var totalCount = await _priorityRepository.GetCountAsync(
            input.Filter
        );

        return new PagedResultDto<PriorityDto>(totalCount,
            ObjectMapper.Map<List<Priority>, List<PriorityDto>>(priorities)
        );
    }

    [Authorize(WorkShopManagementPermissions.Priorities.Create)]
    public async Task<PriorityDto> CreateAsync(CreatePriorityDto input)
    {
        try
        {
            var priority = await _priorityManager.CreateAsync(
            input.Number,
            input.Description
        );

            await _priorityRepository.InsertAsync(priority);
            return ObjectMapper.Map<Priority, PriorityDto>(priority);
        }
        catch (PriorityAlreadyExistsException)
        {
            throw new UserFriendlyException($"A priority with number {input.Number} already exists.");
        }
    }

    [Authorize(WorkShopManagementPermissions.Priorities.Edit)]
    public async Task UpdateAsync(Guid id, UpdatePriorityDto input)
    {
        var priority = await _priorityRepository.GetAsync(id);
        try
        {
            await _priorityManager.ChangeNumberAsync(priority, input.Number);
        }
        catch (PriorityAlreadyExistsException)
        {
            throw new UserFriendlyException("A priority with this number already exists.");
        }
        priority.ChangeDescription(input.Description);
        await _priorityRepository.UpdateAsync(priority);
    }

    [Authorize(WorkShopManagementPermissions.Priorities.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _priorityRepository.DeleteAsync(id);
    }
}
