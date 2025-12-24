using Volo.Abp.Modularity;

namespace WorkShopManagement;

[DependsOn(
    typeof(WorkShopManagementApplicationModule),
    typeof(WorkShopManagementDomainTestModule)
)]
public class WorkShopManagementApplicationTestModule : AbpModule
{

}
