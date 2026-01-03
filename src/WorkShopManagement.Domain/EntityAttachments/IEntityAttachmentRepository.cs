using System;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.EntityAttachments
{
    public interface IEntityAttachmentRepository : IRepository<EntityAttachment, Guid>
    {
    }
}
