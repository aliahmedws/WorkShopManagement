using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.CheckLists;

public interface ICheckListAppService : IApplicationService
{
    Task<PagedResultDto<CheckListDto?>> GetListAsync(GetCheckListListDto input);

    Task<CheckListDto> GetAsync(Guid id);

    Task<CheckListDto> CreateAsync(CreateCheckListDto input);

    Task<CheckListDto> UpdateAsync(Guid id, UpdateCheckListDto input);

    Task DeleteAsync(Guid id);
}
