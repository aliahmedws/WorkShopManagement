using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using WorkShopManagement.RadioOptions;

namespace WorkShopManagement.Controllers.RadioOptions;

[RemoteService(IsEnabled = true)]
[ControllerName("RadioOptions")]
[Area("app")]
[Route("api/app/radio-options")]
public class RadioOptionController : AbpController, IRadioOptionAppService
{
    private readonly IRadioOptionAppService _appService;

    public RadioOptionController(IRadioOptionAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<List<RadioOptionDto>> GetListAsync(GetRadioOptionListDto input)
        => _appService.GetListAsync(input);

    [HttpPost]
    public Task<List<RadioOptionDto>> UpsertAsync(UpsertRadioOptionsDto input)
        => _appService.UpsertAsync(input);

    //[HttpPost]
    //public Task<List<RadioOptionDto>> CreateAsync(CreateRadioOptionDto input)
    //    => _appService.CreateAsync(input);

    //[HttpPut("{id}")]
    //public Task<RadioOptionDto> UpdateAsync(Guid id, UpdateRadioOptionDto input)
    //    => _appService.UpdateAsync(id, input);

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
        => _appService.DeleteAsync(id);
}
