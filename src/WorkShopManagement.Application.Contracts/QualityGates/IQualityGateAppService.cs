using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.QualityGates;

public interface IQualityGateAppService : IApplicationService
{
    Task<QualityGateDto> GetAsync(Guid id);
    Task<QualityGateDto> CreateAsync(CreateQualityGateDto input);
    Task<QualityGateDto> UpdateAsync(UpdateQualityGateDto input, Guid id);
    Task DeleteAsync(Guid id);
    Task<List<QualityGateDto>> GetListAsync();
}
