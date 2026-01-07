using System;
using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace WorkShopManagement.Identity;

public static class UserExtensions
{
    public static bool GetEnforceTwoFactor(this IdentityUser user)
    {
        return user?.GetProperty<bool?>(UserConsts.EnforceTwoFactorPropertyName) ?? false;
    }
}
