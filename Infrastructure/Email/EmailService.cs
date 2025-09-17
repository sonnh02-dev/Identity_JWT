using Identity_JWT.Application.Abstractions.Email;
using System.Net.Mail;
using System.Net;

namespace Identity_JWT.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;

        public EmailService(string smtpHost, int smtpPort, string fromEmail, string password, bool enableSsl = true)
        {
            _fromEmail = fromEmail;
            _smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = enableSsl
            };
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string resetLink)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_fromEmail, "MyApp Support"),
                Subject = "Reset your password",
                Body = $@"
                <h2>Password Reset</h2>
                <p>Click the link below to reset your password:</p>
                <a href='{resetLink}'>Reset Password</a>
                <p>If you did not request this, please ignore.</p>
            ",
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            await _smtpClient.SendMailAsync(mail);
        }
        public async Task SendConfirmEmailAsync(string toEmail, string confirmLink)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_fromEmail, "MyApp Support"),
                Subject = "Confirm your email",
                Body = $@"
            <h2>Email Confirmation</h2>
            <p>Thank you for registering. Please confirm your email by clicking the link below:</p>
            <a href='{confirmLink}'>Confirm Email</a>
            <p>If you did not create this account, you can safely ignore this email.</p>
        ",
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            await _smtpClient.SendMailAsync(mail);
        }

    }
}