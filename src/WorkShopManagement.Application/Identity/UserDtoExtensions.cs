using System;
using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace WorkShopManagement.Identity;

public static class UserDtoExtensions
{
    public static bool GetEnforceTwoFactor(this IdentityUserDto user)
    {
        return user?.GetProperty<bool?>(UserConsts.EnforceTwoFactorPropertyName) ?? false;
    }
}
