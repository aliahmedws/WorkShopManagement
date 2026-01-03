using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WorkShopManagement.EntityFrameworkCore;

namespace WorkShopManagement.EntityAttachments
{
    public class EfCoreEntityAttachmentRepository : EfCoreRepository<WorkShopManagementDbContext, EntityAttachment, Guid>, IEntityAttachmentRepository
    {
        public EfCoreEntityAttachmentRepository(IDbContextProvider<WorkShopManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<List<EntityAttachment>> GetListByEntityAsync(
        Guid entityId,
        EntityType entityType)
        {
            return await (await GetQueryableAsync())
                .Where(x => x.EntityId == entityId && x.EntityType.Equals(entityType))
                .ToListAsync();
        }

        public async Task<IQueryable<EntityAttachment>> GetQueryableByEntityAsync(
            Guid entityId,
            EntityType entityType)
        {

            var queryable = await GetQueryableAsync();
            return queryable.Where(x => x.EntityId == entityId && x.EntityType.Equals(entityType));
        }
    }
}
