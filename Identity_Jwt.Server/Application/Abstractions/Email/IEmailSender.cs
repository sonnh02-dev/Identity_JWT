using FluentResults;

namespace Identity_Jwt.Server.Application.Abstractions.Email
{
    public interface IEmailSender
    {
        Task<Result> SendEmailConfirmationAsync(string toEmail, string confirmationLink);
        Task<Result> SendPasswordResetAsync(string toEmail, string resetLink);
        Task<Result> SendChangeEmailAsync(string toEmail, string changeEmailLink);
        Task<Result> SendTwoFactorCodeAsync(string toEmail, string otpCode);
        Task<Result> SendAccountLockedAsync(string toEmail, int failedAttempts);

    }
}
