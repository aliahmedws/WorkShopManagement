using Volo.Abp.Settings;

namespace WorkShopManagement.Settings;

public class WorkShopManagementSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(WorkShopManagementSettings.MySetting1));
    }
}
