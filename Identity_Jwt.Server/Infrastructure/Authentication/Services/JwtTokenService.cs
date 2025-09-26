using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity_Jwt.Server.Domain.Entities;
using Microsoft.Extensions.Configuration;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Identity_Jwt.Server.Domain.IRepositories;
using Identity_Jwt.Server.Infrastructure.Authentication.Entities;
using Identity_Jwt.Server.Infrastructure.Authentication.Settings;
using Identity_Jwt.Server.Application.Abstractions.Authentication;

namespace Identity_Jwt.Server.Infrastructure.Authentication.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly UserManager<UserAccount> _userManager;

        public JwtTokenService(IOptions<JwtSettings> jwtOptions, IRefreshTokenRepository refreshTokenRepo, UserManager<UserAccount> userManager)
        {
            _jwtSettings = jwtOptions.Value;
            _refreshTokenRepo = refreshTokenRepo;
            _userManager = userManager;
        }

        public async Task<Result<string>> GenerateAccessToken(UserAccount user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var expiryMinutes = _jwtSettings.AccessTokenExpiryMinutes;
            if (expiryMinutes <= 0 || expiryMinutes > 1440)
            {
                return Result.Fail(new Error("Access token expiry minutes must be between 1 and 1440 !")
                    .WithMetadata("errorCode", "InvalidExpiryDays")
                    .WithMetadata("errorType", "BadRequest"));
            }
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds);

            return Result.Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }


        public async Task<Result<string>> GenerateRefreshTokenAsync(int userId)
        {
            var expiryDays = _jwtSettings.RefreshTokenExpiryDays;
            if (expiryDays <= 0 || expiryDays > 30)
            {
                return Result.Fail(new Error("Refresh token expiry days must be between 1 and 30")
                    .WithMetadata("errorCode", "InvalidExpiryDays")
                    .WithMetadata("errorType", "BadRequest"));
            }
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserAccountId = userId,
                ExpiryDays = expiryDays,
                ExpiryDate = DateTime.UtcNow.AddDays(expiryDays)
            };

            var addResult = await _refreshTokenRepo.AddAsync(refreshToken);
            if (addResult.IsFailed)
                return Result.Fail(addResult.Errors);

            return Result.Ok(refreshToken.Token);
        }

        public async Task<Result> RevokeAllUserTokensAsync(int userId)
        {
            return await _refreshTokenRepo.RevokeAllUserTokensAsync(userId);
        }



        public async Task<Result<string>> RefreshAccessTokenAsync(string refreshToken)
        {
            var getResult = await _refreshTokenRepo.GetAsync(refreshToken);

            if (getResult.IsFailed)
                return Result.Fail<string>(getResult.Errors);

            var user = await _userManager.FindByIdAsync(getResult.Value.UserAccountId.ToString());
            if (user == null)
            {
                return Result.Fail(new Error("User not found or has been deleted")
                    .WithMetadata("errorCode", "UserNotFound")
                    .WithMetadata("errorType", "Unauthorized"));
            }
            return await GenerateAccessToken(user);

        }



    }
}
