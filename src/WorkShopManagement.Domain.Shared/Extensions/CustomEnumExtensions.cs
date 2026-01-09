using System;
using Volo.Abp;

namespace WorkShopManagement.Extensions;

public static class CustomEnumExtensions
{
    public static TEnum EnsureDefined<TEnum>(this TEnum value, string? fieldName = null) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new UserFriendlyException($"Invalid value for {fieldName ?? typeof(TEnum).Name}");
        }
        return value;
    }
}
