using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using Volo.Abp.Account.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Sms;
using WorkShopManagement.Localization;

namespace WorkShopManagement.Account.Phone;

public class AccountPhoneService : IAccountPhoneService, ITransientDependency
{
    protected IStringLocalizer<WorkShopManagementResource> Localizer { get; }
    protected ISmsSender SmsSender { get; }

    public AccountPhoneService(ISmsSender smsSender, IStringLocalizer<WorkShopManagementResource> localizer)
    {
        Localizer = localizer;
        SmsSender = smsSender;
    }

    public virtual async Task SendConfirmationCodeAsync(IdentityUser user, string confirmationToken)
    {
        var name = string.IsNullOrWhiteSpace(user.Name) ?
            user.UserName
            : $"{user.Name}{user.Surname?.EnsureStartsWith(' ')}";

        await SmsSender.SendAsync(user.PhoneNumber, Localizer["PhoneConfirmationSms", name, confirmationToken]);
    }

    public virtual async Task SendSecurityCodeAsync(IdentityUser user, string code)
    {
        await SmsSender.SendAsync(user.PhoneNumber, Localizer["EmailSecurityCodeBody", code]);
    }
}
