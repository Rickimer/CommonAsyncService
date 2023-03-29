using EmailService.Shared;

namespace EmailService
{
    public interface IEmailService
    {
        Task SendActivationMail(EmailMessageDto emailMessage);
        Task SendMail(EmailMessageDto emailMessage);
    }
}
