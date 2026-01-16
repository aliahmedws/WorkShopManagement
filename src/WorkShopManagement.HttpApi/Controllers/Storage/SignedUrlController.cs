using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using WorkShopManagement.Storage;

namespace WorkShopManagement.Controllers.Storage;

[RemoteService]
[ControllerName("SignedUrl")]
[Area("app")]
[Route("signed-url")]
public class SignedUrlController(ISignedUrlAppService service) : WorkShopManagementController, ISignedUrlAppService
{
    [HttpGet("sign")]
    public Task<string> GetSignedReadUrlAsync(string gcsUriOrObjectName)
    {
        return service.GetSignedReadUrlAsync(gcsUriOrObjectName);
    }
}
