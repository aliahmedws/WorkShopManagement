using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Polly;
using System;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Google;
using Volo.Abp.Caching;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.MultiTenancy;

namespace WorkShopManagement;

[DependsOn(
    typeof(WorkShopManagementDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpCachingModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpEmailingModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpOpenIddictDomainModule),
    typeof(AbpTenantManagementDomainModule),
    typeof(AbpBlobStoringModule),
    typeof(AbpBlobStoringGoogleModule)
    )]
public class WorkShopManagementDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        ConfigureBlobStoring(context);

#if DEBUG
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif
    }

    private void ConfigureBlobStoring(ServiceConfigurationContext context)
    {
        context.Services.ConfigureOptions<ConfigureGoogleStorageOptions>();

        var configuration = context.Services.GetConfiguration();
        var googleOptions = configuration
            .GetSection(GoogleStorageOptions.ConfigurationKey)
            .Get<GoogleStorageOptions>()
            ?? throw new InvalidOperationException($"Missing config: {GoogleStorageOptions.ConfigurationKey}");

        Configure<AbpBlobStoringOptions>(options =>
        {
            options.Containers.ConfigureDefault(container =>
            {
                container.IsMultiTenant = false;
                container.UseGoogle(google =>
                {
                    google.ClientEmail = googleOptions.ClientEmail;
                    google.ProjectId = googleOptions.ProjectId;
                    google.PrivateKey = googleOptions.PrivateKey;
                    google.Scopes = googleOptions.Scopes;
                    google.ContainerName = googleOptions.ContainerName;
                    google.CreateContainerIfNotExists = false;
                });
            });
        });
    }
}
