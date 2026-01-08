using Microsoft.AspNetCore.Authentication;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using WorkShopManagement.Account;

namespace WorkShopManagement.Pages.Account;

#nullable disable
public abstract class CustomAccountPageModel : AccountPageModel
{
    public ICustomAccountAppService CustomAccountAppService { get; set; }
    public IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache => LazyServiceProvider.LazyGetRequiredService<IdentityDynamicClaimsPrincipalContributorCache>();

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