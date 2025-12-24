using Volo.Abp.Modularity;

namespace WorkShopManagement;

[DependsOn(
    typeof(WorkShopManagementDomainModule),
    typeof(WorkShopManagementTestBaseModule)
)]
public class WorkShopManagementDomainTestModule : AbpModule
{

}
