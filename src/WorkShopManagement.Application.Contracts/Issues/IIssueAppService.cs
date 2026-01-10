using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.Issues;

public interface IIssueAppService : IApplicationService
{
    Task UpsertAsync(Guid carId, UpsertIssuesRequestDto input);
    Task<ListResultDto<IssueDto>> GetListByCarAsync(Guid carId);
    Task<PagedResultDto<IssueListDto>> GetListAsync(GetIssueListInput input);
}
