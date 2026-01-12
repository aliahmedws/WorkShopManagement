using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.LogisticsDetails.ArrivalEstimates
{
    public interface IArrivalEstimateAppService : IApplicationService
    {
        Task<ArrivalEstimateDto> GetAsync(Guid id);
        Task<PagedResultDto<ArrivalEstimateDto>> GetListAsync(Guid logisticsDetailId, PagedAndSortedResultRequestDto input);

        Task<ArrivalEstimateDto> CreateAsync(CreateArrivalEstimateDto input);
        Task<ArrivalEstimateDto> UpdateAsync(Guid id, UpdateArrivalEstimateDto input);
        Task DeleteAsync(Guid id);
        Task<ArrivalEstimateDto?> GetLatestAsync(Guid logisticsDetailId);
    }
}
