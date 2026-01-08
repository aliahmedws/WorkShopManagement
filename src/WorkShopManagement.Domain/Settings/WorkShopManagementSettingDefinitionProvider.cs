using Volo.Abp.Settings;
using WorkShopManagement.Account;

namespace WorkShopManagement.Settings;

public class WorkShopManagementSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        context.Add(new SettingDefinition(CustomAccountSettingNames.TwoFactorLogin.IsRememberBrowserEnabled, bool.FalseString));
    }
}
