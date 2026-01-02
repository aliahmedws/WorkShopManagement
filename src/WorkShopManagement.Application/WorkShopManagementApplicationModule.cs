using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Sms;
using Volo.Abp.TenantManagement;
using WorkShopManagement.External.Shared;

namespace WorkShopManagement;

[DependsOn(
    typeof(WorkShopManagementDomainModule),
    typeof(WorkShopManagementApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpSmsModule)
    )]
public class WorkShopManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.ConfigureExternalProviders();
    }
}
