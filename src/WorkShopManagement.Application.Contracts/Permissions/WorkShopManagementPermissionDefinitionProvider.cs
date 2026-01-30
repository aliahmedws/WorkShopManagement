using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using WorkShopManagement.Localization;

namespace WorkShopManagement.Permissions;

public class WorkShopManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(WorkShopManagementPermissions.GroupName, L("Permission:WorkShopManagement"));

        var bayPermissions = myGroup.AddPermission(WorkShopManagementPermissions.Bays.Default, L("Permission:Bays"));
        bayPermissions.AddChild(WorkShopManagementPermissions.Bays.SetIsActive, L("Permission:SetIsActive"));

        var carPermissions = myGroup.AddPermission(WorkShopManagementPermissions.Cars.Default, L("Permission:Cars"));
        carPermissions.AddChild(WorkShopManagementPermissions.Cars.Create, L("Permission:Create"));
        carPermissions.AddChild(WorkShopManagementPermissions.Cars.Edit, L("Permission:Edit"));
        carPermissions.AddChild(WorkShopManagementPermissions.Cars.Delete, L("Permission:Delete"));
        carPermissions.AddChild(WorkShopManagementPermissions.Cars.Images, L("Permission:Images"));

        var carModelsPermission = myGroup.AddPermission(WorkShopManagementPermissions.CarModels.Default, L("Permission:CarModels"));
        var variantsPermission = myGroup.AddPermission(WorkShopManagementPermissions.Variants.Default, L("Permission:Variants"));

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

        var checkInReportPermissions = myGroup.AddPermission(WorkShopManagementPermissions.CheckInReports.Default, L("Permission:CheckInReports"));
        checkInReportPermissions.AddChild(WorkShopManagementPermissions.CheckInReports.Create, L("Permission:CheckInReports.Create"));
        checkInReportPermissions.AddChild(WorkShopManagementPermissions.CheckInReports.Edit, L("Permission:CheckInReports.Edit"));
        checkInReportPermissions.AddChild(WorkShopManagementPermissions.CheckInReports.Delete, L("Permission:CheckInReports.Delete"));

        var qualityGatesPermission = myGroup.AddPermission(WorkShopManagementPermissions.QualityGates.Default, L("Permission:QualityGates"));
        qualityGatesPermission.AddChild(WorkShopManagementPermissions.QualityGates.Create, L("Permission:QualityGates.Create"));
        qualityGatesPermission.AddChild(WorkShopManagementPermissions.QualityGates.Edit, L("Permission:QualityGates.Edit"));
        qualityGatesPermission.AddChild(WorkShopManagementPermissions.QualityGates.Delete, L("Permission:QualityGates.Delete"));

        var carBaysPermission = myGroup.AddPermission(WorkShopManagementPermissions.CarBays.Default, L("Permission:CarBays"));
        carBaysPermission.AddChild(WorkShopManagementPermissions.CarBays.Create, L("Permission:CarBays.Create"));
        carBaysPermission.AddChild(WorkShopManagementPermissions.CarBays.Edit, L("Permission:CarBays.Edit"));
        carBaysPermission.AddChild(WorkShopManagementPermissions.CarBays.Delete, L("Permission:CarBays.Delete"));
        
        var carBayItemsPermission = myGroup.AddPermission(WorkShopManagementPermissions.CarBayItems.Default, L("Permission:CarBayItems"));
        carBayItemsPermission.AddChild(WorkShopManagementPermissions.CarBayItems.Create, L("Permission:CarBayItems.Create"));
        carBayItemsPermission.AddChild(WorkShopManagementPermissions.CarBayItems.Edit, L("Permission:CarBayItems.Edit"));
        carBayItemsPermission.AddChild(WorkShopManagementPermissions.CarBayItems.Delete, L("Permission:CarBayItems.Delete"));

    
        var productionManagerPermissions = myGroup.AddPermission(WorkShopManagementPermissions.ProductionManager.Default, L("Permission:ProductionManager"));

        var issuePermission = myGroup.AddPermission(WorkShopManagementPermissions.Issues.Default, L("Permission:Issues"));
        issuePermission.AddChild(WorkShopManagementPermissions.Issues.Upsert, L("Permission:Upsert"));

        var logisticsDetailsPermission = myGroup.AddPermission(
            WorkShopManagementPermissions.LogisticsDetails.Default,
            L("Permission:LogisticsDetails")
        );

        logisticsDetailsPermission.AddChild(WorkShopManagementPermissions.LogisticsDetails.Create, L("Permission:LogisticsDetails.Create"));
        logisticsDetailsPermission.AddChild(WorkShopManagementPermissions.LogisticsDetails.Edit, L("Permission:LogisticsDetails.Edit"));
        //logisticsDetailsPermission.AddChild(WorkShopManagementPermissions.LogisticsDetails.Delete, L("Permission:LogisticsDetails.Delete"));

        var arrivalEstimatesPermission = myGroup.AddPermission(
            WorkShopManagementPermissions.ArrivalEstimates.Default,
            L("Permission:ArrivalEstimates")
        );
        arrivalEstimatesPermission.AddChild(WorkShopManagementPermissions.ArrivalEstimates.Create, L("Permission:ArrivalEstimates.Create"));
        arrivalEstimatesPermission.AddChild(WorkShopManagementPermissions.ArrivalEstimates.Edit, L("Permission:ArrivalEstimates.Edit"));
        //arrivalEstimatesPermission.AddChild(WorkShopManagementPermissions.ArrivalEstimates.Delete, L("Permission:ArrivalEstimates.Delete"));

        myGroup.AddPermission(WorkShopManagementPermissions.AuditLogs.Default, L("Permission:AuditLogs"));
        myGroup.AddPermission(WorkShopManagementPermissions.ExportExcel.Default, L("Permission:ExportExcel"));



        var stagesPermission = myGroup.AddPermission(WorkShopManagementPermissions.Stages.Default, L("Permission:Stages"));
        stagesPermission.AddChild(WorkShopManagementPermissions.Stages.Incoming, L("Permission:Stages.Incoming"));
        stagesPermission.AddChild(WorkShopManagementPermissions.Stages.External, L("Permission:Stages.External"));
        stagesPermission.AddChild(WorkShopManagementPermissions.Stages.SCDWarehouse, L("Permission:Stages.SCDWarehouse"));
        stagesPermission.AddChild(WorkShopManagementPermissions.Stages.Production, L("Permission:Stages.Production"));
        stagesPermission.AddChild(WorkShopManagementPermissions.Stages.PostProduction, L("Permission:Stages.PostProduction"));
        stagesPermission.AddChild(WorkShopManagementPermissions.Stages.AwaitingTransport, L("Permission:Stages.AwaitingTransport"));
        stagesPermission.AddChild(WorkShopManagementPermissions.Stages.Dispatched, L("Permission:Stages.Dispatched"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkShopManagementResource>(name);
    }
}
