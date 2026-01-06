using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using WorkShopManagement.Localization;

namespace WorkShopManagement.Permissions;

public class WorkShopManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(WorkShopManagementPermissions.GroupName);

        var carPermissions = myGroup.AddPermission(WorkShopManagementPermissions.Cars.Default, L("Permission:Cars"));
        carPermissions.AddChild(WorkShopManagementPermissions.Cars.Create, L("Permission:Create"));
        carPermissions.AddChild(WorkShopManagementPermissions.Cars.Edit, L("Permission:Edit"));
        carPermissions.AddChild(WorkShopManagementPermissions.Cars.Delete, L("Permission:Delete"));

        var carModelsPermission = myGroup.AddPermission(WorkShopManagementPermissions.CarModels.Default, L("Permission:CarModels"));
        var vehiclesPermission = myGroup.AddPermission(WorkShopManagementPermissions.Vehicles.Default, L("Permission:Vehicles"));

        var checkListsPermission = myGroup.AddPermission(WorkShopManagementPermissions.CheckLists.Default, L("Permission:CheckLists"));
        checkListsPermission.AddChild(WorkShopManagementPermissions.CheckLists.Create, L("Permission:CheckLists.Create"));
        checkListsPermission.AddChild(WorkShopManagementPermissions.CheckLists.Edit, L("Permission:CheckLists.Edit"));
        checkListsPermission.AddChild(WorkShopManagementPermissions.CheckLists.Delete, L("Permission:CheckLists.Delete"));

        var listItemsPermission = myGroup.AddPermission(WorkShopManagementPermissions.ListItems.Default, L("Permission:ListItems"));
        listItemsPermission.AddChild(WorkShopManagementPermissions.ListItems.Create, L("Permission:ListItems.Create"));
        listItemsPermission.AddChild(WorkShopManagementPermissions.ListItems.Edit, L("Permission:ListItems.Edit"));
        listItemsPermission.AddChild(WorkShopManagementPermissions.ListItems.Delete, L("Permission:ListItems.Delete"));


        var radioOptionsPermission = myGroup.AddPermission(WorkShopManagementPermissions.RadioOptions.Default, L("Permission:RadioOptions"));
        radioOptionsPermission.AddChild(WorkShopManagementPermissions.RadioOptions.Create, L("Permission:RadioOptions.Create"));
        radioOptionsPermission.AddChild(WorkShopManagementPermissions.RadioOptions.Edit, L("Permission:RadioOptions.Edit"));
        radioOptionsPermission.AddChild(WorkShopManagementPermissions.RadioOptions.Delete, L("Permission:RadioOptions.Delete"));

        var qualityGatesPermission = myGroup.AddPermission(WorkShopManagementPermissions.QualityGates.Default, L("Permission:QualityGates"));
        qualityGatesPermission.AddChild(WorkShopManagementPermissions.QualityGates.Create, L("Permission:QualityGates.Create"));
        qualityGatesPermission.AddChild(WorkShopManagementPermissions.QualityGates.Edit, L("Permission:QualityGates.Edit"));
        qualityGatesPermission.AddChild(WorkShopManagementPermissions.QualityGates.Delete, L("Permission:QualityGates.Delete"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkShopManagementResource>(name);
    }
}
