using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Account.Settings;
using Volo.Abp.Account.Web;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace WorkShopManagement.Pages.Account;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(LoginModel), typeof(CustomLoginModel))]
public class CustomLoginModel : LoginModel
{
    public CustomLoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<AbpAccountOptions> accountOptions,
        IOptions<IdentityOptions> identityOptions,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
        IWebHostEnvironment webHostEnvironment
        ) : base(schemeProvider, accountOptions, identityOptions, identityDynamicClaimsPrincipalContributorCache, webHostEnvironment)
    {
    }

    public override async Task<IActionResult> OnPostAsync(string action)
    {
        await CheckLocalLoginAsync();

        ValidateModel();

        ExternalProviders = await GetExternalProviders();

        EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);

        await ReplaceEmailToUsernameOfInputIfNeeds();

        await IdentityOptions.SetAsync();

        var result = await SignInManager.PasswordSignInAsync(
            LoginInput.UserNameOrEmailAddress,
            LoginInput.Password,
            LoginInput.RememberMe,
            true
        );

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = result.ToIdentitySecurityLogAction(),
            UserName = LoginInput.UserNameOrEmailAddress
        });

        if (result.RequiresTwoFactor)
        {
            return await TwoFactorLoginResultAsync();
        }

        if (result.IsLockedOut)
        {
            Alerts.Warning(L["UserLockedOutMessage"]);
            return Page();
        }

        if (result.IsNotAllowed)
        {
            return await NotAllowedResultAsync();
        }

        if (!result.Succeeded)
        {
            if (LoginInput.UserNameOrEmailAddress == IdentityDataSeedContributor.AdminUserNameDefaultValue &&
                WebHostEnvironment.IsDevelopment())
            {
                var adminUser = await UserManager.FindByNameAsync(IdentityDataSeedContributor.AdminUserNameDefaultValue);
                if (adminUser == null)
                {
                    ShowRequireMigrateSeedMessage = true;
                    return Page();
                }
            }

            Alerts.Danger(L["InvalidUserNameOrPassword"]);
            return Page();
        }

        //TODO: Find a way of getting user's id from the logged in user and do not query it again like that!
        var user = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress) ??
                   await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);

        Debug.Assert(user != null, nameof(user) + " != null");

        // Clear the dynamic claims cache.
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);

        return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
    }

    protected virtual async Task<IActionResult> NotAllowedResultAsync()
    {
        var notAllowedUser = await GetIdentityUser(LoginInput.UserNameOrEmailAddress);

        if (!await UserManager.CheckPasswordAsync(notAllowedUser, LoginInput.Password))
        {
            Alerts.Danger(L["InvalidUserNameOrPassword"]);
            return Page();
        }

        if (notAllowedUser.ShouldChangePasswordOnNextLogin ||
            await UserManager.ShouldPeriodicallyChangePasswordAsync(notAllowedUser))
        {
            Alerts.Danger(L["LoginIsNotAllowed"]);
            return Page();
        }

        if (notAllowedUser.IsActive)
        {
            await StoreConfirmUser(notAllowedUser);
            return RedirectToPage("./ConfirmUser", new
            {
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash
            });
        }

        Alerts.Danger(L["LoginIsNotAllowed"]);
        return Page();
    }


    protected virtual async Task<IdentityUser> GetIdentityUser(string userNameOrEmailAddress)
    {
        //TODO: Find a way of getting user's id from the logged in user and do not query it again like that!
        var user = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress) ??
            await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);
        Debug.Assert(user != null, nameof(user) + " != null");

        return user;
    }

    protected virtual async Task StoreConfirmUser(IdentityUser user)
    {
        var identity = new ClaimsIdentity(ConfirmUserModel.ConfirmUserScheme);
        identity.AddClaim(new Claim(AbpClaimTypes.UserId, user.Id.ToString()));
        if (user.TenantId.HasValue)
        {
            identity.AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()));
        }
        await HttpContext.SignInAsync(ConfirmUserModel.ConfirmUserScheme, new ClaimsPrincipal(identity));
    }
}
