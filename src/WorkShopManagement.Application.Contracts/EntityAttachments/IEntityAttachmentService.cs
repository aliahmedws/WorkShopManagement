using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.EntityAttachments;

public interface IEntityAttachmentService : IApplicationService
{
    Task<List<EntityAttachmentDto>> GetListAsync(EntityType entityType, List<Guid> entityIds);
    Task<List<EntityAttachmentDto>> GetListAsync(GetEntityAttachmentListDto input);
    Task<List<EntityAttachmentDto>> CreateAsync(CreateAttachmentDto input);
    Task<List<EntityAttachmentDto>> UpdateAsync(UpdateEntityAttachmentDto input);
    Task DeleteAsync(Guid entityId, EntityType entityType);
    Task DeleteManyAsync(EntityType entityType, List<Guid> entityIds);
}
