using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.LogisticsDetails
{
    public interface ILogisticsDetailAppService : IApplicationService
    {
        Task<LogisticsDetailDto> GetAsync(Guid id);
        Task<LogisticsDetailDto?> GetByCarIdAsync(Guid carId);
        Task<PagedResultDto<LogisticsDetailDto>> GetListAsync(PagedAndSortedResultRequestDto input, string? filter = null, Guid? carId = null);
        Task<LogisticsDetailDto> CreateAsync(CreateLogisticsDetailDto input);
        Task<LogisticsDetailDto> UpdateAsync(Guid id, UpdateLogisticsDetailDto input);
        Task DeleteAsync(Guid id);
    }   
}
