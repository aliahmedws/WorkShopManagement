using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Settings;
using WorkShopManagement.Account.Phone;

namespace WorkShopManagement.Account;

public class CustomAccountAppService : AccountAppService, ICustomAccountAppService
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
        (await UserManager.SetTwoFactorEnabledAsync(user, enabled: true)).CheckErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangePhoneNumber
        });

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.TwoFactorEnabled
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

    public virtual async Task<List<string>> GetTwoFactorProvidersAsync(GetTwoFactorProvidersInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        if (await UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor), input.Token))
        {
            var providers = (await UserManager.GetValidTwoFactorProvidersAsync(user)).ToList();

            //TODO: Uncomment for enabling Authenticator.
            //if (!user.HasAuthenticator())
            //{
            //    providers.RemoveAll(x => x == TwoFactorProviderConsts.Authenticator);
            //}
            //TODO: Remove for enabling Authenticator
            providers.RemoveAll(x => x == TwoFactorProviderConsts.Authenticator);
            return providers;
        }

        throw new UserFriendlyException(L["Volo.Account:InvalidUserToken"]);
    }

    public virtual async Task SendTwoFactorCodeAsync(SendTwoFactorCodeInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        if (await UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor), input.Token))
        {
            switch (input.Provider)
            {
                case TwoFactorProviderConsts.Email:
                    {
                        //TODO: Enable if implementing Email Verification
                        //var code = await UserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                        //await AccountEmailer.SendEmailSecurityCodeAsync(user, code);
                        return;
                    }
                case TwoFactorProviderConsts.Phone:
                    {
                        var code = await UserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
                        await PhoneService.SendSecurityCodeAsync(user, code);
                        return;
                    }
                case TwoFactorProviderConsts.Authenticator:
                    {
                        // No need to send code. The client will use the TOTP generator.
                        return;
                    }

                default:
                    throw new UserFriendlyException(L["Volo.Account:UnsupportedTwoFactorProvider"]);
            }
        }

        throw new UserFriendlyException(L["Volo.Account:InvalidUserToken"]);
    }
}
