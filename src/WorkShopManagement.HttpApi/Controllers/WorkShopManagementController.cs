using WorkShopManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace WorkShopManagement.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class WorkShopManagementController : AbpControllerBase
{
    protected WorkShopManagementController()
    {
        LocalizationResource = typeof(WorkShopManagementResource);
    }
}
