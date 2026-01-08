using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Uow;
using WorkShopManagement.Account;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WorkShopManagement.Pages.Account;

#nullable disable
public class SendSecurityCodeModel : CustomAccountPageModel
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

    public List<SelectListItem> Providers { get; set; }

    [BindProperty]
    public string SelectedProvider { get; set; }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnGetAsync()
    {
        var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return RedirectToPage("./Login");
        }

        CheckCurrentTenant(user.TenantId);
        //TODO: CheckCurrentTenant(await SignInManager.GetVerifiedTenantIdAsync()); ???

        Providers = [.. (await CustomAccountAppService.GetTwoFactorProvidersAsync(new GetTwoFactorProvidersInput
        {
            UserId = user.Id,
            Token = await UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor))
        })).Select(userProvider => new SelectListItem
        {
            Text = userProvider,
            Value = userProvider
        })];

        return Page();
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnPostAsync()
    {
        var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        CheckCurrentTenant(user.TenantId);
        //TODO: CheckCurrentTenant(await SignInManager.GetVerifiedTenantIdAsync()); ???

        await CustomAccountAppService.SendTwoFactorCodeAsync(new SendTwoFactorCodeInput()
        {
            UserId = user.Id,
            Provider = SelectedProvider,
            Token = await UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor))
        });

        return RedirectToPage("./VerifySecurityCode", new {
            provider = SelectedProvider,
            returnUrl = ReturnUrl,
            returnUrlHash = ReturnUrlHash,
            rememberMe = RememberMe
        });
    }
}
