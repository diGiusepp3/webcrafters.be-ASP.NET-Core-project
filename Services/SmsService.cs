// FILE: Services/SmsService.cs
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace webcrafters.be_ASP.NET_Core_project.Services
{
    public class SmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _messagingServiceSid;

        public SmsService(IConfiguration config)
        {
            _accountSid = config["Twilio:AccountSid"] ?? throw new ArgumentNullException("Twilio:AccountSid");
            _authToken = config["Twilio:AuthToken"] ?? throw new ArgumentNullException("Twilio:AuthToken");
            _messagingServiceSid = config["Twilio:MessagingServiceSid"] ?? throw new ArgumentNullException("Twilio:MessagingServiceSid");

            TwilioClient.Init(_accountSid, _authToken);
        }

        public void SendSms(string toPhone, string body)
        {
            MessageResource.Create(
                to: new PhoneNumber(toPhone),
                messagingServiceSid: _messagingServiceSid,
                body: body
            );
        }
    }
}