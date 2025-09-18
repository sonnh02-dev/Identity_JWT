using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Identity_JWT.Infrastructure.Email
{
    public class MailMessageFactory
    {
        private readonly string _fromEmail;
        private readonly string _displayName;

        public MailMessageFactory(IOptions<EmailSettings> options, string displayName)
        {
            var settings = options.Value;
            _fromEmail = settings.FromEmail;
            _displayName = settings.DisplayName;
        }

        private MailMessage CreateBaseMessage(string toEmail, string subject, string body)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_fromEmail, _displayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);
            return mail;
        }

        public MailMessage CreateResetPasswordMessage(string toEmail, string resetLink)
        {
            var body = $@"
                <h2>Password Reset</h2>
                <p>Click the link below to reset your password:</p>
                <a href='{resetLink}'>Reset Password</a>
                <p>If you did not request this, please ignore.</p>
            ";

            return CreateBaseMessage(toEmail, "Reset your password", body);
        }

        public MailMessage CreateConfirmEmailMessage(string toEmail, string confirmLink)
        {
            var body = $@"
                <h2>Email Confirmation</h2>
                <p>Thank you for registering. Please confirm your email by clicking the link below:</p>
                <a href='{confirmLink}'>Confirm Email</a>
                <p>If you did not create this account, you can safely ignore this email.</p>
            ";

            return CreateBaseMessage(toEmail, "Confirm your email", body);
        }

        public MailMessage CreateChangeEmailMessage(string toEmail, string confirmLink)
        {
            var body = $@"
                <h2>Email Change Request</h2>
                <p>We received a request to change your account email to this address.</p>
                <p>Please confirm the change by clicking the link below:</p>
                <a href='{confirmLink}'>Confirm Email Change</a>
                <p>If you did not request this change, you can safely ignore this email.</p>
            ";

            return CreateBaseMessage(toEmail, "Confirm your new email address", body);
        }
    }
}
