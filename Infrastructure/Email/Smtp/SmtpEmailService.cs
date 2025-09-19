// Infrastructure/Email/EmailService.cs
using Identity_JWT.Application.Abstractions.Email;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using FluentResults;
using Azure.Core;
using Identity_Jwt.Application.DTOs.Requests;
using Identity_Jwt.Infrastructure.Email.Smtp;
using Identity_Jwt.Infrastructure.Email;

namespace Identity_JWT.Infrastructure.Email.Smtp
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public SmtpEmailService(IOptions<SmtpEmailSettings> options)
        {
            var settings = options.Value;
            _smtpClient = new SmtpClient(settings.SmtpHost, settings.SmtpPort)
            {
                Credentials = new NetworkCredential(settings.FromEmail, settings.Password),
                EnableSsl = settings.EnableSsl
            };
        }

        public async Task<Result> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                await _smtpClient.SendMailAsync(message.ToMailMessage(),cancellationToken);
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