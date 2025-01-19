using Org.BouncyCastle.Asn1.Crmf;
using System.Net.Http.Headers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Voice;
using Twilio.Types;

namespace MahataCrm.Service
{
    public class WhatsappService : IWhatsappService
    {

        private readonly string apiEndpoint = "https://www.waboxapp.com/api/send/chat";
        private readonly string token = "71449ee8f6bc510b9def7bd8c7b7dd396779beb38aafd";
        private readonly long uid = 23793700371;
        private readonly string custom_uid = "msg-1971";

        public WhatsappService()
        {
            // Initialisation de Twilio avec Account SID et Auth Token
           //TwilioClient.Init(accountSid, authToken);
        }

        public async void EnvoyerNotificationWhatsApp(long toWhatsAppNumber, string message)
        {
            using (var client = new HttpClient())
            {
                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token", token),
                    new KeyValuePair<string, string>("uid", uid.ToString()),
                    new KeyValuePair<string, string>("to", toWhatsAppNumber.ToString()),
                    new KeyValuePair<string, string>("custom_uid", custom_uid),
                    new KeyValuePair<string, string>("text", message)
                });

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var resp = await client.PostAsync(apiEndpoint, requestContent);
                var responseString = await resp.Content.ReadAsStringAsync();
                Console.WriteLine($"Message envoyé avec succès, SID : {responseString}");
            }
            
        }

    }
}
