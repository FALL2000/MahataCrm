using System.Net.Mail;
using System.Net;
using Quartz;

namespace MahataCrm.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpConfig = _configuration.GetSection("Smtp");
            using (var client = new SmtpClient(smtpConfig["Host"], int.Parse(smtpConfig["Port"])))
            {
                //client.EnableSsl = bool.Parse(smtpConfig["EnableSsl"]);

                client.Credentials = new NetworkCredential("mahitatanzania@gmail.com", "jpti hdbs dvql bxvh");
                client.Timeout = 300000;
                client.EnableSsl = true; // Use SSL if required by your SMTP server

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpConfig["UserName"]),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);


                client.Send(mailMessage);
            }

            return Task.CompletedTask;
        }

    }
}
