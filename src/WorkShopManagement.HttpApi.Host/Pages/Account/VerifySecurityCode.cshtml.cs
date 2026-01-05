using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Uow;
using WorkShopManagement.Account;

namespace WorkShopManagement.Pages.Account;

#nullable disable
public class VerifySecurityCodeModel : CustomAccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public bool RememberMe { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string Provider { get; set; }

    [BindProperty]
    public string Code { get; set; }

    [BindProperty]
    public bool RememberBrowser { get; set; }

    public bool IsRememberBrowserEnabled { get; protected set; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor;

    public VerifySecurityCodeModel(ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        CurrentPrincipalAccessor = currentPrincipalAccessor;
    }

    [UnitOfWork]
    public virtual async Task OnGetAsync()
    {
        var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            throw new UserFriendlyException(L["VerifySecurityCodeNotLoggedInErrorMessage"]);
        }

        CheckCurrentTenant(user.TenantId);
        //TODO: CheckCurrentTenant(await SignInManager.GetVerifiedTenantIdAsync());

        IsRememberBrowserEnabled = await IsRememberBrowserEnabledAsync();
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnPostAsync()
    {
        //TODO: CheckCurrentTenant(await SignInManager.GetVerifiedTenantIdAsync());

        await IdentityOptions.SetAsync();

        var result = await SignInManager.TwoFactorSignInAsync(
            Provider,
            Code,
            RememberMe,
            await IsRememberBrowserEnabledAsync() && RememberBrowser
        );

        if (result.Succeeded)
        {
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            
            // Clear the dynamic claims cache.
            await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
 
            return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
        }

        if (result.IsLockedOut)
        {
            Alerts.Warning(L["LockedOut"]);
            return Page();
        }

        if (result.IsNotAllowed)
        {
            Alerts.Warning(L["LoginIsNotAllowed"]);
            return Page();
        }

        Alerts.Warning(L["InvalidSecurityCode"]);

        return Page();
    }

    protected virtual async Task<bool> IsRememberBrowserEnabledAsync()
    {
        return await SettingProvider.IsTrueAsync(CustomAccountSettingNames.TwoFactorLogin.IsRememberBrowserEnabled);
    }
}
