using FluentResults;
using Identity_Jwt.Server.Application.DTOs.Requests;
using Identity_Jwt.Server.Application.DTOs.Responses;
using Identity_Jwt.Server.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;

namespace Identity_Jwt.Server.Application.Abstractions.Authentication
{
    public interface IAuthenticationService
    {
        Task<Result> RegisterAsync(RegisterRequest request);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);

        Task<Result<object>> LoginWithPasswordAsync(LoginRequest request);
        Task<Result<object>> LoginWithTwoFactorAsync(LoginWithTwoFactorRequest request);
        Task<AuthenticationProperties> GetExternalLoginPropertiesAsync(string provider, string returnUrl);
        Task<Result<object>> ExternalLoginCallbackAsync();
        Task LogoutAsync(ClaimsPrincipal principal);


        Task<Result> RequestPasswordResetAsync(string email);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);





    }
}
