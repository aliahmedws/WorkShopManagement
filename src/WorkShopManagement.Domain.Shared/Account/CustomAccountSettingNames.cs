namespace WorkShopManagement.Account;

public static class CustomAccountSettingNames
{
    public const string RequireConfirmedPhoneNumber = "Abp.Identity.SignIn.RequireConfirmedPhoneNumber";
    public static class TwoFactorLogin
    {
        public const string IsRememberBrowserEnabled = "Abp.Account.TwoFactorLogin.IsRememberBrowserEnabled";
    }
}
