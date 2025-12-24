using WorkShopManagement.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace WorkShopManagement.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(WorkShopManagementEntityFrameworkCoreModule),
    typeof(WorkShopManagementApplicationContractsModule)
)]
public class WorkShopManagementDbMigratorModule : AbpModule
{
}
