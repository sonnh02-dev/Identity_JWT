using FluentResults;
using Identity_Jwt.Server.Application.DTOs.Requests;
using Identity_Jwt.Server.Application.DTOs.Responses;
using System.Security.Claims;

public interface IUserAccountService
{
    // ------------------------------- Profile & Basic Info -------------------------------
    /// <summary>Get the current authenticated user as a specific response type.</summary>
    Task<TResponse> GetCurrentUserAsync<TResponse>(ClaimsPrincipal principal) where TResponse : class;

    /// <summary>Get user information by user ID.</summary>
    Task<Result<UserAccountResponse>> GetUserByIdAsync(string userId);

    /// <summary>Get user information by email.</summary>
    Task<Result<UserAccountResponse>> GetUserByEmailAsync(string email);

    // Potential helper methods (commented out for now):
    // string GetUserIdAsync(ClaimsPrincipal principal);
    // string GetEmailAsync(ClaimsPrincipal principal);
    // string GetUserNameAsync(ClaimsPrincipal principal);
    // Task<string?> GetPhoneNumberAsync(ClaimsPrincipal principal);
    // IEnumerable<string> GetRolesAsync(ClaimsPrincipal principal);
    // Task<Result<bool>> IsEmailConfirmedAsync(string email);
    // Task<Result<bool>> HasPasswordAsync(ClaimsPrincipal principal);


    // ------------------------------- Password Management -------------------------------
    /// <summary>Add a password when the user registered via an external provider (Google/Facebook, etc.).</summary>
    Task<Result> AddPasswordAsync(ClaimsPrincipal principal, string newPassword);

    /// <summary>Change the current user's password.</summary>
    Task<Result> ChangePasswordAsync(ClaimsPrincipal principal, string oldPassword, string newPassword);


    // ------------------------------- Email Management -------------------------------
    /// <summary>Request email change (sends confirmation link).</summary>
    Task<Result> RequestChangeEmailAsync(ClaimsPrincipal principal, string newEmail);

    /// <summary>Confirm email change using a token.</summary>
    Task<Result> ChangeEmailAsync(ClaimsPrincipal principal, ChangeEmailRequest request);


    // ------------------------------- Phone Number Management -------------------------------
    /// <summary>Request phone number change (sends OTP).</summary>
    Task<Result> RequestChangePhoneNumberAsync(ClaimsPrincipal principal, string newPhoneNumber);

    /// <summary>Confirm phone number change using OTP.</summary>
    Task<Result> ChangePhoneNumberAsync(ClaimsPrincipal principal, ChangePhoneNumberRequest request);

    /// <summary>Directly set a phone number without verification (rarely used).</summary>
    Task<Result> SetPhoneNumberAsync(ClaimsPrincipal principal, string phoneNumber);


    // ------------------------------- Two-Factor Authentication -------------------------------
    /// <summary>Enable two-factor authentication for the current user.</summary>
    Task<Result> EnableTwoFactorAsync(ClaimsPrincipal principal);
}
