// Infrastructure/Email/EmailService.cs
using Identity_JWT.Application.Abstractions.Email;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using FluentResults;

namespace Identity_JWT.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService(IOptions<EmailSettings> options)
        {
            var settings = options.Value;
            _smtpClient = new SmtpClient(settings.SmtpHost, settings.SmtpPort)
            {
                Credentials = new NetworkCredential(settings.FromEmail, settings.Password),
                EnableSsl = settings.EnableSsl
            };
        }

        public async Task<Result> SendEmailAsync(MailMessage mail)
        {
            try
            {
                await _smtpClient.SendMailAsync(mail);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail("Failed to send email")
                             .WithError(ex.Message);
            }
        }
    }
}
