using FluentResults;
using Identity_Jwt.Server.Application.DTOs.Requests;
using Identity_Jwt.Server.Application.DTOs.Responses;
using Identity_Jwt.Server.Application.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;
using Mapster;
using Identity_Jwt.Server.Application.Abstractions.Email;
using Identity_Jwt.Server.Infrastructure.Authentication.Entities;
using Identity_Jwt.Server.Application.Abstractions.Authentication;

namespace Identity_Jwt.Server.Infrastructure.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<UserAccount> _userManager;
    private readonly SignInManager<UserAccount> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    private readonly ILinkFactory _linkFactory;

    public AuthenticationService(
        UserManager<UserAccount> userManager,
        SignInManager<UserAccount> signInManager,
        ITokenService tokenService,
        IConfiguration configuration,
          IEmailSender emailSender,
          ILinkFactory linkFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _configuration = configuration;
        _emailSender = emailSender;
        _linkFactory = linkFactory;
    }

    // -------------------- Register & Confirm Email --------------------
    // Register and send link confirm email
    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        var user = request.Adapt<UserAccount>();

        var createResult = await _userManager.CreateAsync(user, request.Password);//Available check unique email

        if (!createResult.Succeeded)
            return Result.Fail(createResult.Errors.Select(e =>
                new Error(e.Description)
                    .WithMetadata("errorCode", e.Code)
                    .WithMetadata("errorType", "Validation")));

        await _userManager.AddToRoleAsync(user, "User");
        return await RequestEmailConfirmationAsync(user);
    }

    private async Task<Result> RequestEmailConfirmationAsync(UserAccount user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = _linkFactory.GenerateEmailConfirmationLink(user.Email!, token);
        return await _emailSender.SendEmailConfirmationAsync(user.Email!, confirmationLink);
    }


    // Verify token and set 'EmailConfirmed = true' if token is valid
    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Fail(UserErrors.NotFound);

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var confirmResult = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!confirmResult.Succeeded)
            return Result.Fail(confirmResult.Errors.Select(e =>
                new Error(e.Description)
                    .WithMetadata("errorCode", e.Code)
                    .WithMetadata("errorType", "Validation")));

        return Result.Ok();
    }





    // ---------------------------------- Login & JWT ----------------------------------------


    public async Task<Result<LoginResponse>> LoginWithPasswordAsync(LoginRequest request)
    {
        // Validate email & password
        var validationResult = await ValidateUserCredentialsAsync(request);
        if (validationResult.IsFailed)
            return Result.Fail<LoginResponse>(validationResult.Errors);

        var user = validationResult.Value;

        // If 2FA enabled → send code and return RequiresTwoFactor = true
        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            var twoFactorCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var sendResult = await _emailSender.SendTwoFactorCodeAsync(user.Email!, twoFactorCode);
            return sendResult.IsSuccess ?
                 Result.Ok(new LoginResponse(user.Email!, RequiresTwoFactor: true)) :
                 Result.Fail(sendResult.Errors);
        }

        // Otherwise generate tokens directly
        return await GenerateTokenForUserAsync(user);
    }

    private async Task<Result<UserAccount>> ValidateUserCredentialsAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Fail<UserAccount>(UserErrors.NotFound);

        // Ensure email is confirmed
        if (!user.EmailConfirmed)
            return Result.Fail<UserAccount>(UserErrors.EmailNotConfirmed);

        // Verify password
        var checkResult = await _signInManager.CheckPasswordSignInAsync(
               user,
               request.Password,
               lockoutOnFailure: true //enables lockout count
           );
        if (checkResult.IsLockedOut)
        {
            var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);

            await _emailSender.SendAccountLockedAsync(user.Email!, failedAttempts);

            return Result.Fail(UserErrors.AccountLocked);
        }
        await _userManager.ResetAccessFailedCountAsync(user);
        return Result.Ok(user);
    }

    public async Task<Result<LoginResponse>> LoginWithTwoFactorAsync(string email, string twoFactorCode)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Fail(UserErrors.NotFound);

        // Validate 2FA code
        var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", twoFactorCode);
        if (!isValid)
            return Result.Fail(AuthErrors.InvalidTwoFactorCode);

        // Generate tokens if valid
        return await GenerateTokenForUserAsync(user);
    }

    private async Task<Result<LoginResponse>> GenerateTokenForUserAsync(UserAccount user)
    {
        // Generate access token
        var accessTokenResult = await _tokenService.GenerateAccessToken(user);
        if (accessTokenResult.IsFailed)
            return Result.Fail(accessTokenResult.Errors);

        // Generate refresh token
        var refreshTokenResult = await _tokenService.GenerateRefreshTokenAsync(user.Id);
        if (refreshTokenResult.IsFailed)
            return Result.Fail(refreshTokenResult.Errors);

        return Result.Ok(new LoginResponse(user.Email!, accessTokenResult.Value, refreshTokenResult.Value));
    }






    // -------------------- Logout --------------------
    public async Task LogoutAsync(ClaimsPrincipal principal)
    {
        var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _tokenService.RevokeAllUserTokensAsync(userId);
    }





    // -------------------- Reset Password --------------------
    //Send reset link via email to  reset password
    public async Task<Result> RequestPasswordResetAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Fail(UserErrors.NotFound);
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = _linkFactory.GeneratePasswordResetLink(email, token);
        return await _emailSender.SendPasswordResetAsync(email, resetLink);
    }


    //Confirm token to reset password
    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Fail(UserErrors.NotFound);

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
        var resetResult = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

        if (!resetResult.Succeeded)
            return Result.Fail(resetResult.Errors.Select(e =>
                new Error(e.Description)
                    .WithMetadata("errorCode", e.Code)
                    .WithMetadata("errorType", "Validation")));

        return Result.Ok();
    }
}
