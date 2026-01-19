using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.CarBays;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.Bays;

[RemoteService(false)]
[Authorize(WorkShopManagementPermissions.Bays.Default)]
public class BayAppService : ApplicationService, IBayAppService
{
    private readonly IRepository<Bay, Guid> _bayRepository;
    private readonly IRepository<CarBay, Guid> _carBayRepository;

    public BayAppService(IRepository<Bay, Guid> bayRepository, IRepository<CarBay, Guid> carBayRepository)
    {
        _bayRepository = bayRepository;
        _carBayRepository = carBayRepository;
    }

    public async Task<ListResultDto<BayDto>> GetListAsync(GetBayListInput input)
    {
        var queryable = await _bayRepository.GetQueryableAsync();

        input.Filter = input.Filter?.Trim();
        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(x => x.Name.Contains(input.Filter));
        }

        List<Bay> items;

        if (!string.IsNullOrWhiteSpace(input.Sorting) && !input.Sorting.Contains("name"))
        {
            items = await queryable.OrderBy(input.Sorting).ToListAsync();
        }
        else
        {
            items = await queryable.ToListAsync();

            if (input.Sorting != null && input.Sorting.Contains("desc"))
            {
                items = [.. items
                    .OrderByDescending(x => BayHelper.GetNamePrefix(x.Name))
                    .ThenByDescending(x => BayHelper.GetTrailingNumberOrMax(x.Name))
                    .ThenByDescending(x => x.Name)];
            }
            else
            {
                items = [.. items
                    .OrderBy(x => BayHelper.GetNamePrefix(x.Name))
                    .ThenBy(x => BayHelper.GetTrailingNumberOrMax(x.Name))
                    .ThenBy(x => x.Name)];
            }
        }

        var dtos = ObjectMapper.Map<List<Bay>, List<BayDto>>(items);
        return new ListResultDto<BayDto>(dtos);
    }

    [Authorize(WorkShopManagementPermissions.Bays.SetIsActive)]
    public async Task SetIsActiveAsync(Guid id, bool isActive)
    {
        var bay = await _bayRepository.GetAsync(id);

        if (isActive)
        {
            var inUse = await _carBayRepository.AnyAsync(cb => cb.BayId == id && cb.IsActive == true);
            if (inUse)
            {
                throw new UserFriendlyException("This bay has a vehicle in progress");
            }
        }

        bay.SetActive(isActive);
        await _bayRepository.UpdateAsync(bay);
    }
}
