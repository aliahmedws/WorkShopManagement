using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using WorkShopManagement.CarModels;
using WorkShopManagement.ModelCategories;

namespace WorkShopManagement.Controllers.ModelCategories;

[RemoteService(IsEnabled = true)]
[ControllerName("ModelCategories")]
[Area("app")]
[Route("api/app/model-category")]
public class ModelCategoryController : AbpController , IModelCategoryAppService
{
    private readonly IModelCategoryAppService _appService;

    public ModelCategoryController(IModelCategoryAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<ModelCategoryDto>> GetListAsync(GetModelCategoryListDto input)
        => _appService.GetListAsync(input);

}
