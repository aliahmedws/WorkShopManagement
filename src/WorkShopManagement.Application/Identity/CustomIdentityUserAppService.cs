using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.SettingManagement;
using WorkShopManagement.Account;

namespace WorkShopManagement.Identity;

public class CustomIdentityUserAppService : IdentityUserAppService, IIdentityUserAppService
{
    private readonly ISettingManager _settingManager;

    public CustomIdentityUserAppService(
        IdentityUserManager userManager,
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        IOptions<IdentityOptions> identityOptions,
        IPermissionChecker permissionChecker,
        ISettingManager settingManager
        ) : base(userManager, userRepository, roleRepository, identityOptions, permissionChecker)
    {
        _settingManager = settingManager;
    }

    public override async Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
    {
        var created = await base.CreateAsync(input);
        return await EnforceTwoFactorIfRequiredAsync(created);
    }

    public override async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto input)
    {
        var updated = await base.UpdateAsync(id, input);
        return await EnforceTwoFactorIfRequiredAsync(updated);
    }

    private async Task<IdentityUserDto> EnforceTwoFactorIfRequiredAsync(IdentityUserDto dto)
    {
        var user = await UserManager.GetByIdAsync(dto.Id);

        var enforceTwoFactor = user.GetEnforceTwoFactor();
        _ = bool.TryParse(await _settingManager.GetOrNullForUserAsync(CustomAccountSettingNames.RequireConfirmedPhoneNumber, user.Id), out var requireConfirmedPhoneNumber);

        if (enforceTwoFactor != requireConfirmedPhoneNumber)
        {
            await _settingManager.SetForUserAsync(user.Id, CustomAccountSettingNames.RequireConfirmedPhoneNumber, enforceTwoFactor.ToString());
        }

        if (enforceTwoFactor != user.TwoFactorEnabled)
        {
            (await UserManager.SetTwoFactorEnabledAsync(user, enforceTwoFactor)).CheckErrors();
        }

        return dto;
    }
}