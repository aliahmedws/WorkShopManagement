using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WorkShopManagement.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class WorkShopManagementDbContextFactory : IDesignTimeDbContextFactory<WorkShopManagementDbContext>
{
    public WorkShopManagementDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        WorkShopManagementEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<WorkShopManagementDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new WorkShopManagementDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../WorkShopManagement.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
