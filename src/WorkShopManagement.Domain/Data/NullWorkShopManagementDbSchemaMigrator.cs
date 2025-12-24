using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace WorkShopManagement.Data;

/* This is used if database provider does't define
 * IWorkShopManagementDbSchemaMigrator implementation.
 */
public class NullWorkShopManagementDbSchemaMigrator : IWorkShopManagementDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
