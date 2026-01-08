using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Localization;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;
using WorkShopManagement.Identity;

namespace WorkShopManagement;

public static class WorkShopManagementModuleExtensionConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            ConfigureExistingProperties();
            ConfigureExtraProperties();
        });
    }

    private static void ConfigureExistingProperties()
    {
        /* You can change max lengths for properties of the
         * entities defined in the modules used by your application.
         *
         * Example: Change user and role name max lengths

           AbpUserConsts.MaxNameLength = 99;
           IdentityRoleConsts.MaxNameLength = 99;

         * Notice: It is not suggested to change property lengths
         * unless you really need it. Go with the standard values wherever possible.
         *
         * If you are using EF Core, you will need to run the add-migration command after your changes.
         */
    }

    private static void ConfigureExtraProperties()
    {
        /**See the documentation for more:
         *https://abp.io/docs/latest/framework/architecture/modularity/extending/module-entity-extensions
         */

        ObjectExtensionManager.Instance.Modules()
           .ConfigureIdentity(identity =>
           {
               identity.ConfigureUser(user =>
               {
                   user.AddOrUpdateProperty<bool>(
                       UserConsts.EnforceTwoFactorPropertyName,
                       property =>
                       {
                           property.Attributes.Remove(new RequiredAttribute());
                           property.UI.OnCreateForm.IsVisible = true;
                           property.UI.OnEditForm.IsVisible = true;
                       }
                   );
               });
           });
    }
}
