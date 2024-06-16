using AccountService.Model;

namespace AccountService.Services.Mail;

public interface IEmailService
{
    Task<bool> SendEmail(EmailModel email);
}