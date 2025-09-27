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
using Microsoft.AspNetCore.Authentication;
using IAuthenticationService = Identity_Jwt.Server.Application.Abstractions.Authentication.IAuthenticationService;

namespace Identity_Jwt.Server.Infrastructure.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<UserAccount> _userManager;
    private readonly SignInManager<UserAccount> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    private readonly ILinkFactory _linkFactory;
    private readonly IWebHostEnvironment _env;

    public AuthenticationService(
        UserManager<UserAccount> userManager,
        SignInManager<UserAccount> signInManager,
        ITokenService tokenService,
        IConfiguration configuration,
          IEmailSender emailSender,
          ILinkFactory linkFactory,
          IWebHostEnvironment env)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _configuration = configuration;
        _emailSender = emailSender;
        _linkFactory = linkFactory;
        _env = env;
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

    //**Internal login with email & password**
    public async Task<Result<object>> LoginWithPasswordAsync(LoginRequest request)
    {
        // Validate email & password
        var validationResult = await ValidateUserCredentialsAsync(request);
        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        var user = validationResult.Value;

        // If 2FA enabled → send code and return RequiresTwoFactor = true
        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            var twoFactorCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var sendResult = await _emailSender.SendTwoFactorCodeAsync(user.Email!, twoFactorCode);
            return sendResult.IsSuccess ?
                 Result.Ok<object>(new LoginRequiresTwoFactorResponse(
                     user.Email!, 
                     TwoFactorCode: _env.IsDevelopment() ? twoFactorCode : null)) :
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
        if (!checkResult.Succeeded)
        {
            //Wrong password => decrease number of remaining login 
            var maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
            var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
            var remaining = maxAttempts - failedAttempts;
            return Result.Fail<UserAccount>(UserErrors.InvalidPassword(remaining));
        }

        await _userManager.ResetAccessFailedCountAsync(user);
        return Result.Ok(user);
    }

    public async Task<Result<object>> LoginWithTwoFactorAsync(LoginWithTwoFactorRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Result.Fail(UserErrors.NotFound);

        // Validate 2FA code
        var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", request.TwoFactorCode);
        if (!isValid)
            return Result.Fail(AuthErrors.InvalidTwoFactorCode);

        // Generate tokens if valid
        return await GenerateTokenForUserAsync(user);
    }

    private async Task<Result<object>> GenerateTokenForUserAsync(UserAccount user)
    {
        // Generate access token
        var accessTokenResult = await _tokenService.GenerateAccessToken(user);
        if (accessTokenResult.IsFailed)
            return Result.Fail(accessTokenResult.Errors);

        // Generate refresh token
        var refreshTokenResult = await _tokenService.GenerateRefreshTokenAsync(user.Id);
        if (refreshTokenResult.IsFailed)
            return Result.Fail(refreshTokenResult.Errors);

        return Result.Ok<object>(new LoginSuccessResponse(
            user.Email!,
            accessTokenResult.Value,
            refreshTokenResult.Value)
            );
    }

    public Task<AuthenticationProperties> GetExternalLoginPropertiesAsync(string provider, string returnUrl)
    {
        var redirectUrl = $"{_configuration.GetSection("Url:Server")}/account/external-login-callback?returnUrl={returnUrl}";
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Task.FromResult(properties);

    }
    //**External login with Google, Facebook, etc.**
    public async Task<Result<object>> ExternalLoginCallbackAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return Result.Fail("External login info not found");

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

        UserAccount user;
        if (signInResult.Succeeded)
        {
            user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        }
        else
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return Result.Fail("Email not found from external provider");

            user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new UserAccount { UserName = email, Email = email };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return Result.Fail("Failed to create user");
            }

            // Liên kết external login
            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
                return Result.Fail("Failed to link external login");
        }

        // Tạo JWT token
        return await GenerateTokenForUserAsync(user);
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
