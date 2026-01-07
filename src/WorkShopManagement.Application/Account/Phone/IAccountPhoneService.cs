using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace WorkShopManagement.Account.Phone;

public interface IAccountPhoneService
{
    Task SendConfirmationCodeAsync(IdentityUser user, string confirmationToken);

    Task SendSecurityCodeAsync(IdentityUser user, string code);
}
