using Volo.Abp.Modularity;

namespace WorkShopManagement;

public abstract class WorkShopManagementApplicationTestBase<TStartupModule> : WorkShopManagementTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
