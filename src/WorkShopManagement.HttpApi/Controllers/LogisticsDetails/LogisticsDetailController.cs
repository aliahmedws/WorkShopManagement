using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.LogisticsDetails;

namespace WorkShopManagement.Controllers.LogisticsDetails
{
    [ControllerName("LogisticsDetail")]
    [Area("app")]
    [Route("api/app/logistics-details")]
    public class LogisticsDetailController(ILogisticsDetailAppService logisticsDetailAppService)
    : WorkShopManagementController, ILogisticsDetailAppService
    {

        [HttpPost]
        public Task<LogisticsDetailDto> CreateAsync(CreateLogisticsDetailDto input)
        {
            return logisticsDetailAppService.CreateAsync(input);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return logisticsDetailAppService.DeleteAsync(id);
        }

        [HttpGet("{id}")]
        public Task<LogisticsDetailDto> GetAsync(Guid id)
        {
            return logisticsDetailAppService.GetAsync(id);
        }

        [HttpGet("by-car/{carId}")]
        public Task<LogisticsDetailDto?> GetByCarIdAsync(Guid carId)
        {
            return logisticsDetailAppService.GetByCarIdAsync(carId);
        }

        [HttpGet]
        public Task<PagedResultDto<LogisticsDetailDto>> GetListAsync(
            PagedAndSortedResultRequestDto input,
            string? filter = null,
            Guid? carId = null)
        {
            return logisticsDetailAppService.GetListAsync(input, filter, carId);
        }


        [HttpPut("{id}")]
        public Task<LogisticsDetailDto> UpdateAsync(Guid id, UpdateLogisticsDetailDto input)
        {
            return logisticsDetailAppService.UpdateAsync(id, input);
        }


        [HttpPost("{id}/submit-cre")]
        public Task<LogisticsDetailDto> SubmitCreStatusAsync(Guid id)
        {
            return logisticsDetailAppService.SubmitCreStatusAsync(id);
        }

        [HttpPost("{id}/add-or-update-deliver-details")]
        public Task<LogisticsDetailDto> AddOrUpdateDeliverDetailsAsync(Guid id, AddOrUpdateDeliverDetailDto input)
        {
            return logisticsDetailAppService.AddOrUpdateDeliverDetailsAsync(id, input);
        }
    }
}
