using FluentResults;
using Identity_Jwt.Server.Infrastructure.Authentication.Entities;
using System.Security.Claims;

namespace Identity_Jwt.Server.Application.Abstractions.Authentication
{
    public interface ITokenService
    {
        Task<Result<string>> GenerateAccessToken(UserAccount user);
        Task<Result<string>> GenerateRefreshTokenAsync(int userId);
        Task<Result<string>> RefreshAccessTokenAsync(string refreshToken);
          
        Task<Result> RevokeAllUserTokensAsync(int userId);

    }
}