using WorkShopManagement.Localization;
using Volo.Abp.Application.Services;

namespace WorkShopManagement;

/* Inherit your application services from this class.
 */
public abstract class WorkShopManagementAppService : ApplicationService
{
    protected WorkShopManagementAppService()
    {
        LocalizationResource = typeof(WorkShopManagementResource);
    }
}
