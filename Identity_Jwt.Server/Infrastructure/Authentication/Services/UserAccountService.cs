using FluentResults;
using Identity_Jwt.Server.Application.Abstractions.Authentication;
using Identity_Jwt.Server.Application.Abstractions.Email;
using Identity_Jwt.Server.Application.DTOs.Requests;
using Identity_Jwt.Server.Application.DTOs.Responses;
using Identity_Jwt.Server.Application.Errors;
using Identity_Jwt.Server.Infrastructure.Authentication.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Text;

public class UserAccountService : IUserAccountService
{
    private readonly UserManager<UserAccount> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    private readonly ILinkFactory _linkFactory;

    public UserAccountService(
        UserManager<UserAccount> userManager,
        IConfiguration configuration,
        ILinkFactory linkFactory,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _configuration = configuration;
        _linkFactory = linkFactory;
        _emailSender = emailSender;
    }

    private string GetUserIdAsync(ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string GetEmailAsync(ClaimsPrincipal principal)
        => principal.FindFirstValue(JwtRegisteredClaimNames.Email)!;
    private string GetUserNameAsync(ClaimsPrincipal principal)
        => principal.Identity?.Name!;
    private IEnumerable<string> GetRolesAsync(ClaimsPrincipal principal)
        => principal.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
    private async Task<string?> GetPhoneNumberAsync(ClaimsPrincipal principal)
    {
        var user = await _userManager.GetUserAsync(principal);
        return await _userManager.GetPhoneNumberAsync(user!);
    }
    private async Task<Result<bool>> IsEmailConfirmedAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Ok(false);
        return Result.Ok(await _userManager.IsEmailConfirmedAsync(user));
    }
    private async Task<Result<bool>> HasPasswordAsync(ClaimsPrincipal principal)
    {
        var user = await _userManager.GetUserAsync(principal); if (user == null)
            return Result.Ok(false);
        return Result.Ok(await _userManager.HasPasswordAsync(user));
    }
    public async Task<Result<UserAccountResponse>> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result.Fail(UserErrors.NotFound);
        return Result.Ok(user.Adapt<UserAccountResponse>());
    }
    public async Task<Result<UserAccountResponse>> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Fail(UserErrors.NotFound);
        return Result.Ok(user.Adapt<UserAccountResponse>());
    }
    /// <summary>
    /// Get current user from ClaimsPrincipal (only available after login/authorize).
    /// Since ClaimsPrincipal exists only for authenticated users, the user is guaranteed to exist here.
    /// </summary>
    public async Task<TResponse> GetCurrentUserAsync<TResponse>(ClaimsPrincipal principal) where TResponse : class
    {
        var user = await _userManager.GetUserAsync(principal);

        if (typeof(TResponse) == typeof(UserAccount))
            return (user as TResponse)!;

        return user.Adapt<TResponse>();
    }


    // ------------------------------- Password Management -------------------------------
    public async Task<Result> AddPasswordAsync(ClaimsPrincipal principal, string newPassword)
    {
        var user = await GetCurrentUserAsync<UserAccount>(principal);
        var result = await _userManager.AddPasswordAsync(user, newPassword);
        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(e =>
                new Error(e.Description)
                    .WithMetadata("errorCode", e.Code)
                    .WithMetadata("errorType", "Validation")));

        return Result.Ok();
    }

    public async Task<Result> ChangePasswordAsync(ClaimsPrincipal principal, string oldPassword, string newPassword)
    {
        var user = await GetCurrentUserAsync<UserAccount>(principal);

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(e =>
                new Error(e.Description)
                    .WithMetadata("errorCode", e.Code)
                    .WithMetadata("errorType", "Validation")));

        return Result.Ok();
    }

    // ------------------------------- Email Management -------------------------------
    public async Task<Result> RequestChangeEmailAsync(ClaimsPrincipal principal, string newEmail)
    {
        var user = await GetCurrentUserAsync<UserAccount>(principal);

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        var changeEmailLink = _linkFactory.GenerateChangeEmailLink(newEmail, token);

        return await _emailSender.SendChangeEmailAsync(user.Email!, changeEmailLink);
    }

    public async Task<Result> ChangeEmailAsync(ClaimsPrincipal principal, ChangeEmailRequest request)
    {
        var user = await GetCurrentUserAsync<UserAccount>(principal);

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var result = await _userManager.ChangeEmailAsync(user, request.NewEmail, decodedToken);

        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(e =>
                new Error(e.Description)
                    .WithMetadata("errorCode", e.Code)
                    .WithMetadata("errorType", "Validation")));

        var newUserName = request.NewEmail.Split('@')[0];
        if (user.UserName != newUserName)
            await _userManager.SetUserNameAsync(user, newUserName);

        return Result.Ok();
    }

    // ------------------------------- Phone Number Management -------------------------------
    public async Task<Result> RequestChangePhoneNumberAsync(ClaimsPrincipal principal, string newPhoneNumber)
    {
        var user = await GetCurrentUserAsync<UserAccount>(principal);
        var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber);

        // TODO: Integrate SMS provider
        Console.WriteLine($"OTP for {newPhoneNumber}: {token}");

        return Result.Ok();
    }

    public async Task<Result> ChangePhoneNumberAsync(ClaimsPrincipal principal, ChangePhoneNumberRequest request)
    {
        var user = await GetCurrentUserAsync<UserAccount>(principal);
        var result = await _userManager.ChangePhoneNumberAsync(user, request.NewPhoneNumber, request.Token);
        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(e =>
                new Error(e.Description)
                    .WithMetadata("errorCode", e.Code)
                    .WithMetadata("errorType", "Validation")).
                    ToList());

        return Result.Ok();
    }

    public async Task<Result> SetPhoneNumberAsync(ClaimsPrincipal principal, string phoneNumber)
    {
        var user = await GetCurrentUserAsync<UserAccount>(principal);

        var result = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(e =>
                new Error(e.Description)
                    .WithMetadata("errorCode", e.Code)
                    .WithMetadata("errorType", "Validation")));

        return Result.Ok();
    }
    //------------------------------------------------------------------------------------------
    public async Task<Result> EnableTwoFactorAsync(ClaimsPrincipal principal)
    {
        var user = await GetCurrentUserAsync<UserAccount>(principal);
        var result = await _userManager.SetTwoFactorEnabledAsync(user, true);

        return result.Succeeded
            ? Result.Ok()
            : Result.Fail(result.Errors.Select(e => e.Description));
    }

}
