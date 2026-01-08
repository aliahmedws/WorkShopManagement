using System.Threading.Tasks;

namespace WorkShopManagement.External.Twilio;

public interface ITwilioService
{
    Task<TwilioSmsResponseEto?> SendSmsAsync(string to, string body);
}
