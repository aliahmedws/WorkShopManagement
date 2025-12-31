using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using WorkShopManagement.CarModels;

namespace WorkShopManagement.Controllers.CarModels;

[RemoteService(IsEnabled = true)]
[ControllerName("CarModels")]
[Area("app")]
[Route("api/app/car-models")]
public class CarModelController : WorkShopManagementController, ICarModelAppService
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
