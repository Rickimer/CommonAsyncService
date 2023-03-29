using EmailService.Shared;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailService
{
    public class EmailService : IEmailService
    {
        EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendMail(EmailMessageDto emailMessage)
        {
            if (emailMessage == null)
                throw new ArgumentNullException("emailMessage");

            var message = new MimeMessage();
            emailMessage.Subject = emailMessage.Subject ?? string.Empty;
            emailMessage.From = emailMessage.From ?? string.Empty;

            message.From.Add(new MailboxAddress(emailMessage.From, "RickimerSite@yandex.ru"));
            message.To.Add(new MailboxAddress(emailMessage.ToEmail, emailMessage.ToEmail));
            message.Subject = emailMessage.Subject;

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailMessage.Message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 465, true);
                await client.AuthenticateAsync("RickimerSite@yandex.ru", _emailSettings.EmailPassword);
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }

        public async Task SendActivationMail(EmailMessageDto emailMessage)
        {
            if (emailMessage == null)
                throw new ArgumentNullException("emailMessage");
            
            emailMessage.From = emailMessage.From ?? "Администрация сайта";
            emailMessage.Message = $"Follow the <b>link</b> to activate <a href=\"{emailMessage.Message}\">Confirm email</a>";

            await SendMail(emailMessage);
        }
    }
}
