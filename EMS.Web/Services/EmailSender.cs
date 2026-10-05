using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EMS.Web.Services
{
    public class EmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlMessage)
        {
            using var message = new MailMessage();

            message.From = new MailAddress(
                _settings.FromEmail,
                _settings.FromName);

            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = htmlMessage;
            message.IsBodyHtml = true;

            using var smtpClient = new SmtpClient(
                _settings.Host,
                _settings.Port);

            smtpClient.EnableSsl = true;

            smtpClient.Credentials = new NetworkCredential(
                _settings.UserName,
                _settings.Password);

            await smtpClient.SendMailAsync(message);
        }
    }
}