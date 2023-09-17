using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Application.Notifications.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace IAE.Microservice.Infrastructure
{
    public class EmailNotificationSender : INotificationSender
    {
        private readonly SmtpManagement _smtpManagement;

        public EmailNotificationSender(IOptions<SmtpManagement> smtpManagement)
        {
            _smtpManagement = smtpManagement.Value;
        }

        public async Task SendAsync(Message message)
        {            
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_smtpManagement.FromName, _smtpManagement.FromEmail));
            emailMessage.To.Add(new MailboxAddress(message.To));
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message.Body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    host: _smtpManagement.Server,
                    port: _smtpManagement.Port,
                    useSsl: false);

                if (!string.IsNullOrEmpty(_smtpManagement.User) && !string.IsNullOrEmpty(_smtpManagement.Password)) {
                    await client.AuthenticateAsync(_smtpManagement.User, _smtpManagement.Password);
                }
                
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
