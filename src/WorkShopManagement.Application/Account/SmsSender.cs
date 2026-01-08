using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Sms;
using WorkShopManagement.External.Twilio;

namespace WorkShopManagement.Account;

public class SmsSender : ISmsSender, ITransientDependency
{
    private readonly ITwilioService _twilioService;
    public SmsSender(ITwilioService twilioService)
    {
        _twilioService = twilioService;
    }

    public async Task SendAsync(SmsMessage smsMessage)
    {
        await _twilioService.SendSmsAsync(smsMessage.PhoneNumber, smsMessage.Text);
    }
}