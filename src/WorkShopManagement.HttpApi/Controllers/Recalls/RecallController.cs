using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkShopManagement.Recalls;

namespace WorkShopManagement.Controllers.Recalls
{
    [ControllerName("Recall")]
    [Area("app")]
    [Route("api/app/recalls")]
    public class RecallController(IRecallAppService recallAppService) : WorkShopManagementController, IRecallAppService
    {
        [HttpGet("{id}")]
        public Task<RecallDto> GetAsync(Guid id)
        {
            return recallAppService.GetAsync(id);
        }

        [HttpGet("by-car/{carId}")]
        public Task<List<RecallDto>> GetListByCarAsync(Guid carId)
        {
            return recallAppService.GetListByCarAsync(carId);
        }

        [HttpPost]
        public Task<RecallDto> CreateAsync(CreateRecallDto input)
        {
            return recallAppService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public Task<RecallDto> UpdateAsync(Guid id, UpdateRecallDto input)
        {
            return recallAppService.UpdateAsync(id, input);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return recallAppService.DeleteAsync(id);
        }

        [HttpGet("external/{carId}")]
        public Task<List<RecallDto>> GetExternalRecallsAsync(Guid carId)
        {
            return recallAppService.GetExternalRecallsAsync(carId);
        }

        [HttpPost("{carId}/add-or-update-recalls")]
        public Task<List<RecallDto>> AddOrUpdateRecallsAsync(Guid carId, List<RecallDto> inputs)
        {
            return recallAppService.AddOrUpdateRecallsAsync(carId, inputs);
        }
    }
}
