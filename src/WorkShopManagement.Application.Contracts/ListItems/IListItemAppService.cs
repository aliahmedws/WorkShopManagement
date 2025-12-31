using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.ListItems;

public interface IListItemAppService : IApplicationService
{
    Task<PagedResultDto<ListItemDto>> GetListAsync(GetListItemListDto input);
    Task<ListItemDto> GetAsync(Guid id);
    Task<ListItemDto> CreateAsync(CreateListItemDto input);
    Task<ListItemDto> UpdateAsync(Guid id, UpdateListItemDto input);
    Task DeleteAsync(Guid id);
}
