using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace WorkShopManagement.CarModels;

[RemoteService(IsEnabled = true)]
[ControllerName("CarModels")]
[Area("app")]
[Route("api/app/car-models")]
public class CarModelController : AbpController, ICarModelAppService
{
    private readonly ICarModelAppService _appService;

    public CarModelController(ICarModelAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<CarModelDto>> GetListAsync(GetCarModelListDto input)
        => _appService.GetListAsync(input);

}
