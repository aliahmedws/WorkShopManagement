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
    public EntitySubType? SubType { get; private set; }
    public FileAttachment Attachment { get; private set; } = default!;

    private EntityAttachment() { }

    public EntityAttachment(Guid id, Guid entityId, EntityType entityType, EntitySubType? subType, FileAttachment attachment)
        : base(id)
    {
        SetEntityId(entityId);
        SetEntityType(entityType, subType);
        SetAttachment(attachment);
    }
    public void SetEntityType(EntityType entityType, EntitySubType? subType)
    {
        EntityType = Check.NotNull(entityType, nameof(entityType));
        SubType = EntityType.EnsureIsEntitySubType(subType);
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
