using Microsoft.Extensions.Localization;
using WorkShopManagement.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace WorkShopManagement;

[Dependency(ReplaceServices = true)]
public class WorkShopManagementBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<WorkShopManagementResource> _localizer;

    public WorkShopManagementBrandingProvider(IStringLocalizer<WorkShopManagementResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
