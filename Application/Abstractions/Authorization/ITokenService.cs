using System.Security.Claims;

namespace Identity_JWT.Application.Abstractions.Authorization
{
    public interface ITokenService
    {
        string GenerateAccessToken(int userId, string email, IEnumerable<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        bool ValidateToken(string token);

    }
}