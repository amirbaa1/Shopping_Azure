using System.Net;
using System.Net.Mail;
using AccountService.Model;

namespace AccountService.Services.Mail;

public class EmailService : IEmailService
{
    public EmailService(ILogger<EmailService> logger, EmailSetting emailSetting)
    {
        _logger = logger;
        _EmailSetting = emailSetting;
    }

    public EmailSetting _EmailSetting { get; }
    private readonly ILogger<EmailService> _logger;

    public async Task<bool> SendEmail(EmailModel email)
    {
        try
        {
            var message = new MailMessage(email.From, email.To, email.Sub, email.Body);

            using (var emailClient = new SmtpClient(_EmailSetting.HOST, _EmailSetting.PORT))
            {
                emailClient.Credentials = new NetworkCredential(_EmailSetting.User, _EmailSetting.Password);
                await emailClient.SendMailAsync(message);
            }

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error sanding email : {e.Message}");
            return false;
        }
    }
}