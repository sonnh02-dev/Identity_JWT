// Infrastructure/Authorization/AuthorizationService.cs
using Identity_Jwt.Server.Application.Abstractions.Authorization;
using Identity_Jwt.Server.Infrastructure.Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Identity_Jwt.Server.Infrastructure.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly UserManager<UserAccount> _userManager;

        public AuthorizationService(UserManager<UserAccount> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> IsInRoleAsync(ClaimsPrincipal user, string role)
        {
            var appUser = await _userManager.GetUserAsync(user);
            return appUser != null && await _userManager.IsInRoleAsync(appUser, role);
        }

        public async Task<IEnumerable<string>> GetRolesAsync(ClaimsPrincipal user)
        {
            var appUser = await _userManager.GetUserAsync(user);
            return appUser != null ? await _userManager.GetRolesAsync(appUser) : Enumerable.Empty<string>();
        }

        public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission)
        {
            // ví dụ: lấy claim "permission" từ user
            return user.Claims.Any(c => c.Type == "permission" && c.Value == permission);
        }

        public Task<IEnumerable<Claim>> GetUserClaimsAsync(ClaimsPrincipal user)
        {
            return Task.FromResult(user.Claims.AsEnumerable());
        }
    }
}
