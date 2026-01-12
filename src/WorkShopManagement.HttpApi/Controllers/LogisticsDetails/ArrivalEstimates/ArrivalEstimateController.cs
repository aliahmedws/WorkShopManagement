using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.LogisticsDetails.ArrivalEstimates;

namespace WorkShopManagement.Controllers.LogisticsDetails.ArrivalEstimates
{
    [ControllerName("ArrivalEstimate")]
    [Area("app")]
    [Route("api/app/arrival-estimates")]
    public class ArrivalEstimateController(IArrivalEstimateAppService arrivalEstimateAppService)
      : WorkShopManagementController, IArrivalEstimateAppService
    {
        [HttpPost]
        public Task<ArrivalEstimateDto> CreateAsync(CreateArrivalEstimateDto input)
        {
            return arrivalEstimateAppService.CreateAsync(input);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return arrivalEstimateAppService.DeleteAsync(id);
        }

        [HttpGet("{id}")]
        public Task<ArrivalEstimateDto> GetAsync(Guid id)
        {
            return arrivalEstimateAppService.GetAsync(id);
        }

        [HttpGet]
        public Task<PagedResultDto<ArrivalEstimateDto>> GetListAsync(
            [FromQuery] Guid logisticsDetailId,
            [FromQuery] PagedAndSortedResultRequestDto input)
        {
            return arrivalEstimateAppService.GetListAsync(logisticsDetailId, input);
        }

        [HttpGet("latest/{logisticsDetailId}")]
        public Task<ArrivalEstimateDto?> GetLatestAsync(Guid logisticsDetailId)
        {
            return arrivalEstimateAppService.GetLatestAsync(logisticsDetailId);
        }

        [HttpPut("{id}")]
        public Task<ArrivalEstimateDto> UpdateAsync(Guid id, UpdateArrivalEstimateDto input)
        {
            return arrivalEstimateAppService.UpdateAsync(id, input);
        }
    }
}
