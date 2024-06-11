using System.Net;
using System.Net.Mail;
using OrderService.Model;
using OrderService.Model.DTO;

namespace OrderService.Repository.Mail;

public class EmailService : IEmailService
{
    public EmailService(EmailSetting emailSetting, ILogger<EmailService> logger)
    {
        _emailSetting = emailSetting;
        _logger = logger;
    }

    private EmailSetting _emailSetting { get; }
    private readonly ILogger<EmailService> _logger;

    public async Task<bool> sendEmail(Email email)
    {
        try
        {
            var message = new MailMessage(email.From, email.To, email.Subject, email.Body);


            using (var emailClient = new SmtpClient(_emailSetting.HOST, _emailSetting.PORT))
            {
                emailClient.Credentials = new NetworkCredential(_emailSetting.User, _emailSetting.Password);
                await emailClient.SendMailAsync(message);
                _logger.LogInformation($"sand Email : {email.To}");
            }

            ;
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error sanding email : {e.Message}");
            return false;
        }
    }
}