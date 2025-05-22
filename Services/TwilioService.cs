using System.Runtime.CompilerServices;
using AgroMonitor.Services.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AgroMonitor.Services
{
    public class TwilioService : ITwilioService
    {
        private string _twilioPhoneNumber;
        public TwilioService()
        {
            string accountSId = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            _twilioPhoneNumber = Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER");

            if(string.IsNullOrEmpty(accountSId) && string.IsNullOrEmpty(authToken) && string.IsNullOrEmpty(_twilioPhoneNumber))
            {
                throw new InvalidOperationException("Twilio credentials are not configured.");
            }

            TwilioClient.Init(accountSId, authToken);
        }

        public void SendSMS(string message, string phoneNumber)
        {
            var response = MessageResource.Create(
                new PhoneNumber(phoneNumber),
                from: new PhoneNumber(_twilioPhoneNumber),
                body: message
            );
            

            Console.WriteLine(response.Sid);
        }
    }
}
