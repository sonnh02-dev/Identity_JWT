// Infrastructure/Authentication/AuthenticationService.cs
using FluentResults;
using Identity_JWT.Application.Abstractions.Authentication;
using Identity_JWT.Application.Abstractions.Authorization;
using Identity_JWT.Application.Abstractions.Email;
using Identity_JWT.Application.DTOs.Requests;
using Identity_JWT.Application.DTOs.Responses;
using Identity_JWT.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Identity_JWT.Infrastructure.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<UserAuth> _userManager;
        private readonly SignInManager<UserAuth> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly MailMessageFactory _messageFactory;

        public AuthenticationService(
            UserManager<UserAuth> userManager,
            SignInManager<UserAuth> signInManager,
            ITokenService tokenService,
            IEmailService emailService,
            IConfiguration configuration,
            MailMessageFactory messageFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _configuration = configuration;
            _messageFactory = messageFactory;
        }
        //--------------------------------------------Register and confirm email-----------------------------------------------
        public async Task<Result> RegisterAsync(RegisterRequest request)
        {
            var user = new UserAuth { Email = request.Email };//hoặc dùng mapper

            var createResult = await _userManager.CreateAsync(user, request.Password);//Bao gồm check unique email

            if (!createResult.Succeeded)
            {
                return Result.Fail("Registration failed")
                             .WithErrors((IEnumerable<IError>)createResult.Errors);
            }
            await _userManager.AddToRoleAsync(user, "Student");

            return await SendEmailConfirmationAsync(user);

        }
        public async Task<Result> SendEmailConfirmationAsync(UserAuth user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var queryParams = new Dictionary<string, string?>
              {
                 { "email", user.Email },
                 { "token", token }
             };

            var confirmationLink = QueryHelpers.AddQueryString(
                _configuration.GetValue<string>("ServerUrl") + "/authentication/confirm-email",
                queryParams
            );

            var mail = _messageFactory.CreateConfirmEmailMessage(user.Email, confirmationLink);
            var sendResult = await _emailService.SendEmailAsync(mail);

            if (sendResult.IsFailed)
                return Result.Fail("Failed to send confirmation email")
                             .WithErrors(sendResult.Errors);

            return Result.Ok();
        }
        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Fail("User not found.")
                             .WithError(new Error("InvalidRequest"));
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
            {
                return Result.Fail("Email confirmation failed")
                             .WithErrors(confirmResult.Errors.Select(e => new Error(e.Description).WithMetadata("Code", e.Code)));
            }

            return Result.Ok();
        }

        //-----------------------------------------------Login and create JWT token --------------------------------------
        public async Task<Result<TokenResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Fail(new Error("Email not exists !")
                    .WithMetadata("code", "InvalidEmail")
                    .WithMetadata("errorType", "NotFound"));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Result.Fail(new Error("Wrong password !")
                    .WithMetadata("code", "InvalidPassword")
                    .WithMetadata("errorType", "Validation"));

            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateAccessToken(user.Id, request.Email, roles);

            var tokenDto = new TokenResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1)
            };
            //await _signInManager.SignInAsync(user, true);//when use cookie authentication.

            return Result.Ok(tokenDto);
        }

        //--------------------------------------------------Logout------------------------------------------------------
        public Task SignOutAsync() => _signInManager.SignOutAsync();

        // --------------------------------------------------ChangePassword-------------------------------------------    
        public async Task<Result> ChangePasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
            {
                return Result.Fail("User not found.")
                             .WithError(new Error("InvalidRequest"));
            }

            var changeResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!changeResult.Succeeded)
            {
                return Result.Fail("Change password failed")
                             .WithErrors(changeResult.Errors.Select(e => new Error(e.Description)
                             .WithMetadata("Code", e.Code)));
            }

            return Result.Ok();
        }



        //----------------------------------------------ResetPassword-----------------------------------
        public async Task<Result> RequestPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Fail(new Error("Email không tồn tại")
                      .WithMetadata("code", "InvalidEmail")
                      .WithMetadata("errorType", "NotFound"));
            }


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var queryParams = new Dictionary<string, string?>
              {
               { "email", email },
               { "token", token }
              };

            var resetLink = QueryHelpers.AddQueryString(
                _configuration.GetValue<string>("ClientUrl") + "/reset-password",
            queryParams
            );

            var mail = _messageFactory.CreateResetPasswordMessage(email, resetLink);
            var result = await _emailService.SendEmailAsync(mail);

            return Result.Ok();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return Result.Fail("User not found.")
                             .WithError(new Error("InvalidRequest"));
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));

            var resetResult = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (!resetResult.Succeeded)
            {
                return Result.Fail("Reset password failed")
                             .WithErrors(resetResult.Errors.Select(e => new Error(e.Description)
                             .WithMetadata("Code", e.Code)));
            }

            return Result.Ok();
        }



        //public async Task<(IdentityResult Result, string? Token, int? UserId)> GenerateEmailConfirmationAsync(ClaimsPrincipal principal)
        //{
        //    var user = await _userManager.GetUserAsync(principal);

        //    if (user == null)
        //    {
        //        return (IdentityResult.Failed(new IdentityError
        //        {
        //            Code = "InvalidRequest",
        //            Description = "User not found."
        //        }), null, null);
        //    }

        //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    return (IdentityResult.Success, WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)), user.Id);
        //}
        //----------------------------------------------ChangeEmail-----------------------------------

        public async Task<Result> RequestChangeEmailAsync(UserAuth user, string newEmail)
        {
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var queryParams = new Dictionary<string, string?>
            {
                { "userId", user.Id.ToString() },
                { "email", newEmail },
                { "token", encodedToken }
            };

            var changeEmailLink = QueryHelpers.AddQueryString(
                _configuration["ServerUrl"] + "/api/changeemail/confirm",
                queryParams
            );
            var mail = _messageFactory.CreateChangeEmailMessage(user.Email, changeEmailLink);

            var sendResult = await _emailService.SendEmailAsync(mail);

            return sendResult.IsSuccess
                ? Result.Ok()
                : Result.Fail(sendResult.Errors);
        }

        public async Task<Result> ConfirmChangeEmailAsync(int userId, string newEmail, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result.Fail("User not found");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ChangeEmailAsync(user, newEmail, decodedToken);

            if (!result.Succeeded)
                return Result.Fail("Change email failed").WithErrors(result.Errors.Select(e => e.Description));

            // Nếu email được dùng làm UserName thì update luôn
            if (user.UserName != newEmail)
                await _userManager.SetUserNameAsync(user, newEmail);

            return Result.Ok();
        }



        public async Task RefreshSignInAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user != null)
            {
                await _signInManager.RefreshSignInAsync(user);
            }
        }

        public async Task<(IdentityResult Result, string? Token, int? UserId)> GenerateEmailChangeAsync(ClaimsPrincipal principal, string newEmail)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
            {
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidRequest",
                    Description = "User not found."
                }), null, null);
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            return (IdentityResult.Success, WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)), user.Id);
        }

    }
}
