using System.Threading.Tasks;
using Volo.Abp.Account;

namespace WorkShopManagement.Account;

public interface ICustomAccountAppService : IAccountAppService
{
    Task SendPhoneNumberConfirmationTokenAsync(SendPhoneNumberConfirmationTokenDto input);
    Task ConfirmPhoneNumberAsync(ConfirmPhoneNumberInput input);
}
