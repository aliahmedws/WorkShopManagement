using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.EntityAttachments;

public interface IEntityAttachmentAppService : IApplicationService
{
    Task<List<EntityAttachmentDto>> GetListAsync(GetEntityAttachmentListDto input);
    Task<List<EntityAttachmentDto>> CreateAsync(CreateAttachmentDto input);
    Task<List<EntityAttachmentDto>> UpdateAsync(UpdateEntityAttachmentDto input);
    Task DeleteAsync(Guid entityId, EntityType entityType);
}
