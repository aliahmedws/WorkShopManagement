using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;
using WorkShopManagement.ModelReports;

namespace WorkShopManagement.Controllers.ModelReports;

[RemoteService(IsEnabled = true)]
[ControllerName("ModelReport")]
[Area("app")]
[Route("api/app/model-reports")]
public class ModelReportController : AbpController
{

    private readonly IModelReportAppService _service;

    public ModelReportController(IModelReportAppService service)
    {
        _service = service;
    }

    [HttpGet("{carId}/download")]
    public Task<IRemoteStreamContent> DownloadAsync(Guid carId)
    {
        return _service.DownloadAsync(carId);
    }
}
