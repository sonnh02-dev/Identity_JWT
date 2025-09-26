// Application/Abstractions/Authorization/IAuthorizationService.cs
using System.Security.Claims;

namespace Identity_Jwt.Server.Application.Abstractions.Authorization
{
    public interface IAuthorizationService
    {
        Task<bool> IsInRoleAsync(ClaimsPrincipal user, string role);
        Task<IEnumerable<string>> GetRolesAsync(ClaimsPrincipal user);
        Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission);
        Task<IEnumerable<Claim>> GetUserClaimsAsync(ClaimsPrincipal user);
    }
}
