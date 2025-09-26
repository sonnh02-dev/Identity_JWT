using FluentEmail.Core;
using FluentResults;
using Identity_Jwt.Server.Application.Abstractions.Email;
using Identity_Jwt.Server.Application.Errors;

namespace Identity_Jwt.Server.Infrastructure.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly IWebHostEnvironment _env;

        public EmailSender(IFluentEmail fluentEmail, IWebHostEnvironment env)
        {
            _fluentEmail = fluentEmail;
            _env = env;
        }

        private async Task<Result> SendEmailAsync(string toEmail, string subject, string templateFileName, object model)
        {
            try
            {
                var templatePath = Path.Combine(_env.ContentRootPath, "Infrastructure", "Email", "Templates", templateFileName);

                var sendResult = await _fluentEmail
                    .To(toEmail)
                    .Subject(subject)
                    .UsingTemplateFromFile(templatePath, model)
                    .SendAsync();

                return sendResult.Successful
                    ? Result.Ok()
                    : Result.Fail(string.Join(", ", sendResult.ErrorMessages));
            }
            catch (Exception ex)
            {
                return Result.Fail(CommonErrors.InternalServerError($"Failed to send email: {ex.Message}"));
            }
        }

        public Task<Result> SendEmailConfirmationAsync(string toEmail, string confirmationLink)
            => SendEmailAsync(toEmail, "Confirm your email", "EmailConfirmation.cshtml", new { Link = confirmationLink });

        public Task<Result> SendPasswordResetAsync(string toEmail, string resetLink)
            => SendEmailAsync(toEmail, "Reset your password", "PasswordReset.cshtml", new { Link = resetLink });

        public Task<Result> SendChangeEmailAsync(string toEmail, string changeEmailLink)
            => SendEmailAsync(toEmail, "Confirm your new email address", "EmailChange.cshtml", new { Link = changeEmailLink });

        public Task<Result> SendTwoFactorCodeAsync(string toEmail, string twoFactorCode)
            => SendEmailAsync(toEmail, "2FA Verification", "TwoFactorCode.cshtml", new { Code = twoFactorCode });
        public Task<Result> SendAccountLockedAsync(string toEmail,int failedAttempts)
           => SendEmailAsync(toEmail, "Your account has been locked", "AccountLocked.cshtml", new { FailedLoginAttempts = failedAttempts }
    );

    }
}
