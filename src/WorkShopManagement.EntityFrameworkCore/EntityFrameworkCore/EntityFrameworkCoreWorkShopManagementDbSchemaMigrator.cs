using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorkShopManagement.Data;
using Volo.Abp.DependencyInjection;

namespace WorkShopManagement.EntityFrameworkCore;

public class EntityFrameworkCoreWorkShopManagementDbSchemaMigrator
    : IWorkShopManagementDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreWorkShopManagementDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the WorkShopManagementDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<WorkShopManagementDbContext>()
            .Database
            .MigrateAsync();
    }
}
