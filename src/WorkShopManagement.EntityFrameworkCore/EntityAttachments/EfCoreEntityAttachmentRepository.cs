using System;
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
    }
}
