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


        var carModelsPermission = myGroup.AddPermission(WorkShopManagementPermissions.CarModels.Default, L("Permission:CarModels"));

        var checkListsPermission = myGroup.AddPermission(WorkShopManagementPermissions.CheckLists.Default, L("Permission:CheckLists"));
        checkListsPermission.AddChild(WorkShopManagementPermissions.CheckLists.Create, L("Permission:CheckLists.Create"));
        checkListsPermission.AddChild(WorkShopManagementPermissions.CheckLists.Edit, L("Permission:CheckLists.Edit"));
        checkListsPermission.AddChild(WorkShopManagementPermissions.CheckLists.Delete, L("Permission:CheckLists.Delete"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkShopManagementResource>(name);
    }
}
