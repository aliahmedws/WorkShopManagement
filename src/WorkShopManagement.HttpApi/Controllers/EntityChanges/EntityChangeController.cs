using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.EntityChanges;

namespace WorkShopManagement.Controllers.EntityChanges;

[RemoteService]
[ControllerName("EntityChange")]
[Area("app")]
[Route("api/app/entity-changes")]
public class EntityChangeController(IEntityChangeAppService service) : WorkShopManagementController, IEntityChangeAppService
{
    [HttpGet("history/{entityId}/{entityTypeFullName}")]
    public Task<ListResultDto<EntityChangeWithUsernameDto>> GetChangeHistoryAsync(string entityId, string entityTypeFullName)
    {
        return service.GetChangeHistoryAsync(entityId, entityTypeFullName);
    }
}
