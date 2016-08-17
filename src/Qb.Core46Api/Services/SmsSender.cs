using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Clients;
using Twilio.Creators.Api.V2010.Account;
using Twilio.Resources.Api.V2010.Account;
using Twilio.Types;

namespace Qb.Core46Api.Services
{
    public class SmsSender : ISmsSender
    {
        private readonly string _fromNumber;
        private readonly string _sid;
        private readonly string _token;

        public SmsSender(IOptions<SmsSenderOptions> options)
        {
            _sid = options.Value.Sid;
            _token = options.Value.Token;
            _fromNumber = options.Value.FromNumber;
        }

        public async Task<bool> SendSms(string message, string toNumber)
        {
            TwilioClient.Init(_sid, _token);
            var restClient = new TwilioRestClient(_sid, _token);
            var msg =
                await
                    new MessageCreator(_sid, new PhoneNumber(toNumber), new PhoneNumber(_fromNumber), message)
                        .ExecuteAsync(restClient);
            return msg.GetStatus() != MessageResource.Status.FAILED;
        }
    }
}