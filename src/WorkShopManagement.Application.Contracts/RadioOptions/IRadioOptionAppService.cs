using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.RadioOptions;

public interface IRadioOptionAppService : IApplicationService
{
    Task<List<RadioOptionDto>> GetListAsync(GetRadioOptionListDto input);
    Task<List<RadioOptionDto>> UpsertAsync(UpsertRadioOptionsDto input);
    //Task<List<RadioOptionDto>> CreateAsync(CreateRadioOptionDto input);
    //Task<RadioOptionDto> UpdateAsync(Guid id, UpdateRadioOptionDto input);
    Task DeleteAsync(Guid id);
}
