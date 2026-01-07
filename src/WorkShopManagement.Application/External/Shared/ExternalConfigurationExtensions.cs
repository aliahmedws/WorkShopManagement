using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.Modularity;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.External.CarsXE;
using WorkShopManagement.External.Nhtsa;
using WorkShopManagement.External.Vpic;

namespace WorkShopManagement.External.Shared;

public static class ExternalConfigurationExtensions
{
    public static void ConfigureExternalProviders(this ServiceConfigurationContext context)
    {
        ConfigureOptions(context);
    }

    private static void ConfigureOptions(ServiceConfigurationContext context)
    {
        context.Services.ConfigureOptions<ConfigureVpicApiOptions>();
        context.Services.ConfigureOptions<ConfigureNhtsaApiOptions>();
        context.Services.ConfigureOptions<ConfigureBlobStorageOptions>();
        context.Services.ConfigureOptions<ConfigureCarsXeApiOptions>();
    }
}
