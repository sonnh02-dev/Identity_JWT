using System.Net.Mail;
using Identity_Jwt.Application.DTOs.Requests;
using Identity_Jwt.Infrastructure.Email.Smtp;
using Microsoft.Extensions.Options;

namespace Identity_Jwt.Infrastructure.Email
{
    public class EmailMessageFactory
    {
        private readonly string _fromEmail;
        private readonly string _displayName;

        public EmailMessageFactory(IOptions<SmtpEmailSettings> options)
        {
            var settings = options.Value;
            _fromEmail = settings.FromEmail;
            _displayName = settings.DisplayName;
        }

        private EmailMessage CreateBaseRequest(string toEmail, string subject, string body, bool isBodyHtml = true)
        {
            var mail = new EmailMessage
            {
                From = new MailAddress(_fromEmail, _displayName),
                To=toEmail,
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };
            return mail;
        }

        public EmailMessage CreateResetPasswordRequest(string toEmail, string resetLink)
        {
            var body = $@"
                <h2>Password Reset</h2>
                <p>Click the link below to reset your password:</p>
                <a href='{resetLink}'>Reset Password</a>
                <p>If you did not request this, please ignore.</p>
            ";

            return CreateBaseRequest(toEmail, "Reset your password", body);
        }

        public EmailMessage CreateConfirmEmailRequest(string toEmail, string confirmLink)
        {
            var body = $@"
                <h2>Email Confirmation</h2>
                <p>Thank you for registering. Please confirm your email by clicking the link below:</p>
                <a href='{confirmLink}'>Confirm Email</a>
                <p>If you did not create this account, you can safely ignore this email.</p>
            ";

            return CreateBaseRequest(toEmail, "Confirm your email", body);
        }

        public EmailMessage CreateChangeEmailRequest(string toEmail, string confirmLink)
        {
            var body = $@"
                <h2>Email Change Request</h2>
                <p>We received a request to change your account email to this address.</p>
                <p>Please confirm the change by clicking the link below:</p>
                <a href='{confirmLink}'>Confirm Email Change</a>
                <p>If you did not request this change, you can safely ignore this email.</p>
            ";

            return CreateBaseRequest(toEmail, "Confirm your new email address", body);
        }
    }
}