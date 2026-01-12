using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

namespace WorkShopManagement.EntityAttachments;

public static class EntityTypeValidationHelper
{
    // Prefer IReadOnlyDictionary + readonly to avoid runtime mutation.
    public static readonly IReadOnlyDictionary<EntityType, EntitySubType[]> EntityTypeMap
        = new Dictionary<EntityType, EntitySubType[]>
        {
            {
                EntityType.Issue,
                new[]
                {
                    EntitySubType.Issue_Details,
                    EntitySubType.Issue_RectificationAction,
                    EntitySubType.Issue_QualityControl,
                    EntitySubType.Issue_RepairerDetails
                }
            }
        };

    public static EntitySubType? EnsureIsEntitySubType(this EntityType entityType, EntitySubType? entitySubType)
    {
        if (!IsEntitySubType(entityType, entitySubType))
        {
            throw new UserFriendlyException("Invalid entity sub type for this entity type");
        }

        return entitySubType;
    }

    public static bool IsEntitySubType(this EntityType entityType, EntitySubType? entitySubType)
    {
        if (!EntityTypeMap.TryGetValue(entityType, out var allowedSubTypes))
        {
            return true;
        }

        if (allowedSubTypes is null || allowedSubTypes.Length == 0)
        {
            return entitySubType is null;
        }

        return entitySubType is not null && allowedSubTypes.Contains(entitySubType.Value);
    }

    public static EntitySubType[] GetSubTypes(this EntityType type)
    {
        return EntityTypeMap.TryGetValue(type, out var result) ? result ?? [] : [];
    }
}
