using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AspNet8WebApp.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SmtpSettings> smtpsettings;

        public EmailService(IOptions<SmtpSettings> smtpsettings)
        {
            this.smtpsettings = smtpsettings;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string text)
        {

            SmtpClient client = new SmtpClient(smtpsettings.Value.MAIL_HOST)
            {
                Port = smtpsettings.Value.MAIL_PORT,
                Credentials = new NetworkCredential(smtpsettings.Value.MAIL_USERNAME, smtpsettings.Value.MAIL_PASSWORD),
                EnableSsl = true
            };

            // Creating and Sending Email  
            MailMessage message = new MailMessage();
            message.From = new MailAddress(smtpsettings.Value.MAIL_ADDRESS);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = $"<html><body>{text}</body></html>";
            message.IsBodyHtml = true;

            try
            {
                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
