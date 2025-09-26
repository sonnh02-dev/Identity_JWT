namespace Identity_Jwt.Server.Application.Abstractions.Authentication
{
    public interface ILinkFactory
    {
        string GenerateEmailConfirmationLink(string email, string token);
        string GeneratePasswordResetLink(string email, string token);
        string GenerateChangeEmailLink(string newEmail, string token);
    }

}
