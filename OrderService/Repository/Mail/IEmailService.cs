using OrderService.Model;

namespace OrderService.Repository.Mail;

public interface IEmailService
{
    Task<bool> sendEmail(Email email);
}