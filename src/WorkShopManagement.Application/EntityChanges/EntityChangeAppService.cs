using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AuditLogging;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.EntityChanges;

[RemoteService]
[Authorize(WorkShopManagementPermissions.AuditLogs.Default)]
public class EntityChangeAppService : WorkShopManagementAppService, IEntityChangeAppService
{
    private readonly IAuditLogRepository _auditLogRepository;
    public EntityChangeAppService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<ListResultDto<EntityChangeWithUsernameDto>> GetChangeHistoryAsync(string entityId, string entityTypeFullName)
    {
        var entityChanges = await _auditLogRepository.GetEntityChangesWithUsernameAsync(entityId, entityTypeFullName);
        return new ListResultDto<EntityChangeWithUsernameDto>
        {
            Items = ObjectMapper.Map<List<EntityChangeWithUsername>, List<EntityChangeWithUsernameDto>>(entityChanges)
        };
    }
}
