using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Settings;
using WorkShopManagement.Account.Phone;

namespace WorkShopManagement.Account;

public class CustomAccountAppService : AccountAppService
{
    protected IAccountPhoneService PhoneService { get; }

    public CustomAccountAppService(
        IdentityUserManager userManager,
        IIdentityRoleRepository roleRepository,
        IAccountEmailer accountEmailer,
        IdentitySecurityLogManager identitySecurityLogManager,
        IOptions<IdentityOptions> identityOptions,
        IAccountPhoneService phoneService
        ) : base(userManager, roleRepository, accountEmailer, identitySecurityLogManager, identityOptions)
    {
        PhoneService = phoneService;
    }

    public virtual async Task SendPhoneNumberConfirmationTokenAsync(SendPhoneNumberConfirmationTokenDto input)
    {
        await CheckIfPhoneNumberConfirmationEnabledAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);

        if (!input.PhoneNumber.IsNullOrWhiteSpace())
        {
            (await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();
        }

        CheckPhoneNumber(user);

        var token = await UserManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
        await PhoneService.SendConfirmationCodeAsync(user, token);
    }

    public virtual async Task ConfirmPhoneNumberAsync(ConfirmPhoneNumberInput input)
    {
        await CheckIfPhoneNumberConfirmationEnabledAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);

        CheckPhoneNumber(user);

        (await UserManager.ChangePhoneNumberAsync(user, user.PhoneNumber, input.Token)).CheckErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangePhoneNumber
        });
    }

    protected virtual void CheckPhoneNumber(IdentityUser user)
    {
        if (string.IsNullOrEmpty(user.PhoneNumber))
        {
            throw new BusinessException("Volo.Account:PhoneNumberEmpty");
        }
    }

    protected virtual async Task CheckIfPhoneNumberConfirmationEnabledAsync()
    {
        if (!await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.EnablePhoneNumberConfirmation))
        {
            throw new BusinessException("Volo.Account:PhoneNumberConfirmationDisabled");
        }
    }
}
