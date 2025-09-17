// Application/Abstractions/Authentication/IAuthenticationService.cs
using FluentResults;
using Identity_JWT.Application.DTOs.Requests;
using Identity_JWT.Application.DTOs.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;

namespace Identity_JWT.Application.Abstractions.Authentication
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterAsync(RegisterRequest request);
        Task<Result<TokenResponse>> LoginAsync(LoginRequest request);
        Task SignOutAsync();
        Task<(IdentityResult Result, string? Token)> GeneratePasswordResetTokenAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest request);
        Task<IdentityResult> ChangePasswordAsync(ClaimsPrincipal principal, string oldPassword, string newPassword);
        Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<string> GenerateEmailConfirmationTokenAsync(ClaimsPrincipal principal);
        Task<string> GenerateRefreshTokenAsync();
    }
}
