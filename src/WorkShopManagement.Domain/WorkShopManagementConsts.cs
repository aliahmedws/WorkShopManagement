using Volo.Abp.Identity;

namespace WorkShopManagement;

public static class WorkShopManagementConsts
{
    public const string DbTablePrefix = "App";
    public const string? DbSchema = null;
    public const string AdminEmailDefaultValue = "Admin";//IdentityDataSeedContributor.AdminEmailDefaultValue;
    public const string AdminPasswordDefaultValue = "#wsm-SCD*123!"; // IdentityDataSeedContributor.AdminPasswordDefaultValue;
}
