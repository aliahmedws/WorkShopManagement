using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.EntityAttachments;

[Audited]
public class EntityAttachment : FullAuditedAggregateRoot<Guid>
{
    public Guid EntityId { get; private set; }
    public EntityType EntityType { get; private set; }
    public FileAttachment Attachment { get; private set; } = default!;

    private EntityAttachment() { }

    public EntityAttachment(Guid id, Guid entityId, EntityType entityType, FileAttachment attachment)
        : base(id)
    {
        SetEntityId(entityId);
        SetEntityType(entityType);
        SetAttachment(attachment);
    }
    public void SetEntityType(EntityType entityType)
    {
        EntityType = Check.NotNull(entityType, nameof(entityType));
    }

    public void SetEntityId(Guid entityId)
    {
        EntityId = Check.NotDefaultOrNull<Guid>(entityId, nameof(entityId));
    }

    public void SetAttachment(FileAttachment attachment)
    {
        Attachment = Check.NotNull(attachment, nameof(attachment));
    }
}
