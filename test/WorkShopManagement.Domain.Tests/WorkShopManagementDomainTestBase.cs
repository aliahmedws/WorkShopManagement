using Volo.Abp.Modularity;

namespace WorkShopManagement;

/* Inherit from this class for your domain layer tests. */
public abstract class WorkShopManagementDomainTestBase<TStartupModule> : WorkShopManagementTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
