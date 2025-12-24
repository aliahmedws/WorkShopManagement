using WorkShopManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace WorkShopManagement.Permissions;

public class WorkShopManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(WorkShopManagementPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(WorkShopManagementPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkShopManagementResource>(name);
    }
}
