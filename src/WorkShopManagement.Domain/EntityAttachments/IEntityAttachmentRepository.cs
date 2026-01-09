using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.EntityAttachments
{
    public interface IEntityAttachmentRepository : IRepository<EntityAttachment, Guid>
    {
        Task<List<EntityAttachment>> GetListAsync(EntityType entityType, List<Guid> entityIds);
        Task<List<EntityAttachment>> GetListByEntityAsync(
        Guid entityId,
        EntityType entityType);
        Task<IQueryable<EntityAttachment>> GetQueryableByEntityAsync(
            Guid entityId,
            EntityType entityType);


    }
}
