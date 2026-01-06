using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace WorkShopManagement.QualityGates;

[RemoteService(IsEnabled = true)]
[ControllerName("QualityGates")]
[Area("app")]
[Route("api/app/quality-gates")]
public class QualityGateController : AbpController, IQualityGateAppService
{
    private readonly IQualityGateAppService _appService;

    public QualityGateController(IQualityGateAppService appService)
    {
        _appService = appService;
    }

    [HttpPost]
    public async Task<QualityGateDto> CreateAsync(CreateQualityGateDto input)
    {
        return await _appService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _appService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public async Task<QualityGateDto> GetAsync(Guid id)
    {
        return await _appService.GetAsync(id);
    }

    [HttpGet]
    public async Task<List<QualityGateDto>> GetListAsync()
    {
        return await _appService.GetListAsync();
    }

    [HttpPut("{id}")]
    public async Task<QualityGateDto> UpdateAsync(UpdateQualityGateDto input, Guid id)
    {
        return await _appService.UpdateAsync(input, id);
    }
}
