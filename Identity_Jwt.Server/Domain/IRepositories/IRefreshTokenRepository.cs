using FluentResults;
using Identity_Jwt.Server.Domain.Entities;

namespace Identity_Jwt.Server.Domain.IRepositories
{
    public interface IRefreshTokenRepository
    {
        Task<Result> AddAsync(RefreshToken refreshToken);
        Task<Result<RefreshToken>> GetAsync(string refreshToken);
        Task<Result> RevokeAllUserTokensAsync(int userId);
    }
}
