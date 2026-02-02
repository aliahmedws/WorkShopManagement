namespace WorkShopManagement.Permissions;

public static class WorkShopManagementPermissions
{
    public const string GroupName = "WorkShopManagement";

    public static class Bays
    {
        public const string Default = GroupName + ".Bays";
        public const string SetIsActive = Default + ".SetIsActive";
    }

    public static class Cars
    {
        public const string Default = GroupName + ".Cars";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Images = Default + ".Images";
    }

    public static class CarModels
    {
        public const string Default = GroupName + ".CarModels";
    }

    public static class Variants
    {
        public const string Default = GroupName + ".Variants";
    }
    public static class CheckLists
    {
        public const string Default = GroupName + ".CheckLists";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class ListItems
    {
        public const string Default = GroupName + ".ListItems";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class RadioOptions
    {
        public const string Default = GroupName + ".RadioOptions";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }


    public static class  CheckInReports 
    {
        public const string Default = GroupName + ".CheckInReports";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
    public static class QualityGates
    {
        public const string Default = GroupName + ".QualityGates";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class ProductionManager
    {
        public const string Default = GroupName + ".ProductionManager";
    }

    public static class CarBays
    {
        public const string Default = GroupName + ".CarBays";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class CarBayItems
    {
        public const string Default = GroupName + ".CarBayItems";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Issues
    {
        public const string Default = GroupName + ".Issues";
        public const string Upsert = Default + ".Upsert";
    }

    public static class LogisticsDetails
    {
        public const string Default = GroupName + ".LogisticsDetails";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class ArrivalEstimates
    {
        public const string Default = GroupName + ".ArrivalEstimates";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class AuditLogs
    {
        public const string Default = GroupName + ".AuditLogs";
    }

    public static class Recalls
    {
        public const string Default = GroupName + ".Recalls";
    }

    public static class ViewModelReportSummary
    {
        public const string Default = GroupName + ".ViewModelReportSummary";
    }

    public static class Stickers
    {
        public const string Default = GroupName + ".Stickers";
        public const string PrintProductionSticker = Default + ".PrintProductionStickers";
        public const string PrintReceivingSticker = Default + ".PrintReceivingStickers";
    }

    public static class ExportExcel
    {
        public const string Default = GroupName + ".ExportExcel";
    }

    public static class Stages
    {
        public const string Default = GroupName + ".Stages";
        public const string Incoming = Default + ".Incoming";
        public const string External = Default + ".External";
        public const string SCDWarehouse = Default + ".SCDWarehouse";
        public const string Production = Default + ".Production";
        public const string PostProduction = Default + ".PostProduction";
        public const string AwaitingTransport = Default + ".AwaitingTransport";
        public const string Dispatched = Default + ".Dispatched";
    }
}
