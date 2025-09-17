namespace Identity_JWT.Application.Abstractions.Email
{
    public interface IEmailService
    {
        Task SendResetPasswordEmailAsync(string toEmail, string resetLink);
        Task SendConfirmEmailAsync(string toEmail, string confirmLink); 

    }
}
