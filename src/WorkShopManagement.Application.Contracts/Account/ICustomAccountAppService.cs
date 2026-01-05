using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Account;

namespace WorkShopManagement.Account;

public interface ICustomAccountAppService : IAccountAppService
{
    Task SendPhoneNumberConfirmationTokenAsync(SendPhoneNumberConfirmationTokenDto input);
    Task ConfirmPhoneNumberAsync(ConfirmPhoneNumberInput input);
    Task<List<string>> GetTwoFactorProvidersAsync(GetTwoFactorProvidersInput input);
    Task SendTwoFactorCodeAsync(SendTwoFactorCodeInput input);
}
