using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.Bays;

[RemoteService(isEnabled: false)]
public class BayAppService : ApplicationService, IBayAppService
{
    private readonly IRepository<Bay, Guid> _bayRepository;

    public BayAppService(IRepository<Bay,Guid> bayRepository)
    {
        _bayRepository = bayRepository;
    }

    public async Task<ListResultDto<BayDto>> GetListAsync()
    {
        var queryable = await _bayRepository.GetQueryableAsync();

        var items = queryable
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.Name)
            .ToList();

        var dtos = ObjectMapper.Map<List<Bay>, List<BayDto>>(items);

        return new ListResultDto<BayDto>(dtos);
    }
}
