// Application/Abstractions/Authentication/IAuthenticationService.cs
using FluentResults;
using Identity_JWT.Application.DTOs.Requests;
using Identity_JWT.Application.DTOs.Responses;
using Identity_JWT.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;

namespace Identity_JWT.Application.Abstractions.Authentication
{
    public interface IAuthenticationService
    {
        Task<Result> RegisterAsync(RegisterRequest request);
        Task<Result> SendEmailConfirmationAsync(UserAccount user);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);


        Task<Result<TokenResponse>> LoginAsync(LoginRequest request);
        Task SignOutAsync();

        Task<Result> ChangePasswordAsync(ClaimsPrincipal principal, string oldPassword, string newPassword);

        Task<Result> RequestPasswordResetAsync(string email);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);


        //Task<string> GenerateRefreshTokenAsync();

        //-------------------------------------Change Email--------------------------------
        Task<Result> RequestChangeEmailAsync(ClaimsPrincipal principal, string newEmail);
        Task<Result> ConfirmChangeEmailAsync(int userId, string newEmail, string token);

    }
}
