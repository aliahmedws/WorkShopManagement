namespace WorkShopManagement.Permissions;

public static class WorkShopManagementPermissions
{
    public const string GroupName = "WorkShopManagement";

    public static class CarModels
    {
        public const string Default = GroupName + ".CarModels";
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

}
