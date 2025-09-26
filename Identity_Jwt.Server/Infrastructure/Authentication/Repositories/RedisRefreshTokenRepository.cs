using FluentResults;
using Identity_Jwt.Server.Application.Errors;
using Identity_Jwt.Server.Domain.Entities;
using Identity_Jwt.Server.Domain.IRepositories;
using StackExchange.Redis;
using System.Text.Json;
using IDatabase = StackExchange.Redis.IDatabase;


public static class RedisKeys
{
    public static RedisKey RefreshTokenKey(string token) => (RedisKey)$"refresh-token:{token}";
    public static RedisKey UserRefreshTokensKey(int userId) => (RedisKey)$"refresh-tokens:user:{userId}";
}


public class RedisRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDatabase _db;

    public RedisRefreshTokenRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }


    public async Task<Result> AddAsync(RefreshToken refreshToken)
    {
        var refreshTokenKey = RedisKeys.RefreshTokenKey(refreshToken.Token);
        var userTokensKey = RedisKeys.UserRefreshTokensKey(refreshToken.UserAccountId);

        try
        {
            string json = JsonSerializer.Serialize(refreshToken);

            // Lưu token (với TTL)
            await _db.StringSetAsync(refreshTokenKey, json, TimeSpan.FromDays(refreshToken.ExpiryDays));

            // Lưu token ID vào set của user (index để quản lý)
            await _db.SetAddAsync(userTokensKey, refreshToken.Token);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(CommonErrors.InternalServerError($"Failed to save refresh token: {ex.Message}")
                         .CausedBy(ex));

        }
    }



    public async Task<Result<RefreshToken>> GetAsync(string refreshToken)
    {
        var refreshTokenKey = RedisKeys.RefreshTokenKey(refreshToken);

        var value = await _db.StringGetAsync(refreshTokenKey);

        if (value.IsNullOrEmpty)
            return Result.Fail(AuthErrors.RefreshTokenNotFoundOrExpired);


        var tokenEntity = JsonSerializer.Deserialize<RefreshToken>(value!);
        return Result.Ok(tokenEntity);
    }


    public async Task<Result> RevokeAllUserTokensAsync(int userId)
    {
        var userTokensKey = RedisKeys.UserRefreshTokensKey(userId);

        try
        {
            // Lấy toàn bộ token IDs trong set của user
            var tokens = await _db.SetMembersAsync(userTokensKey);
            if (tokens.Length == 0)
            {
                // Không có token => chỉ cần xóa set (nếu tồn tại)
                await _db.KeyDeleteAsync(userTokensKey);
                return Result.Ok().WithSuccess(new Success("No tokens found to revoke"));
            }

            // Dùng batch để xóa token keys + set key cùng lúc
            var batch = _db.CreateBatch();
            var tasks = new List<Task>();

            foreach (var t in tokens)
            {
                tasks.Add(batch.KeyDeleteAsync(RedisKeys.RefreshTokenKey(t)));
            }

            // Xóa luôn set index
            tasks.Add(batch.KeyDeleteAsync(userTokensKey));

            batch.Execute();
            await Task.WhenAll(tasks);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(CommonErrors.InternalServerError($"Failed to revoke user tokens: {ex.Message}")
            .CausedBy(ex));
        }
    }

}
