// Infrastructure/Authentication/AuthenticationService.cs
using FluentResults;
using Identity_JWT.Application.Abstractions.Authentication;
using Identity_JWT.Application.Abstractions.Authorization;
using Identity_JWT.Application.Abstractions.Email;
using Identity_JWT.Application.DTOs.Requests;
using Identity_JWT.Application.DTOs.Responses;
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

        public AuthenticationService(
            UserManager<UserAuth> userManager,
            SignInManager<UserAuth> signInManager,
            ITokenService tokenService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
        }
        //--------------------------------------------Register and confirm email-----------------------------------------------
        public async Task<IdentityResult> RegisterAsync(RegisterRequest request)
        {
            var user = new UserAuth
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);  //Check unique and create 
            if (!result.Succeeded)
                return result;

            // Sinh token xác nhận email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"https://yourapp.com/api/account/confirm-email?userId={user.Id}&token={encodedToken}";
            await _emailService.SendConfirmEmailAsync(user.Email, confirmationLink);


            return IdentityResult.Success;
        }
        public async Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidRequest",
                    Description = "User not found."
                });
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            return await _userManager.ConfirmEmailAsync(user, token);
        }
        //-----------------------------------------------Login and create JWT token --------------------------------------
        public async Task<Result<TokenResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Fail(new Error("Email không tồn tại")
                    .WithMetadata("code", "InvalidEmail")
                    .WithMetadata("errorType", "NotFound"));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Result.Fail(new Error("Sai mật khẩu")
                    .WithMetadata("code", "InvalidPassword")
                    .WithMetadata("errorType", "Validation"));

            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateAccessToken(user.Id, user.Email, roles);

            var tokenDto = new TokenResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1)
            };

            return Result.Ok(tokenDto);
        }

        //--------------------------------------------------Logout------------------------------------------------------
        public Task SignOutAsync() => _signInManager.SignOutAsync();

        // --------------------------------------------------ChangePassword-------------------------------------------    
        public async Task<IdentityResult> ChangePasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidRequest",
                    Description = "User not found."
                });
            }

            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }


        //---------------------------------------------------------------------------------
        public async Task<IdentityResult> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return IdentityResult.Failed(); // hoặc trả về Success nhưng bỏ qua để tránh lộ email

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var resetLink = $"{_appSettings.ClientUrl}/reset-password?email={email}&token={encodedToken}";

            await _emailService.SendResetPasswordEmailAsync(email, resetLink);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidRequest",
                    Description = "User not found."
                });
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
            return await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
        }

        public async Task<(IdentityResult Result, string? Token)> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidRequest",
                    Description = "User not found."
                }), null);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return (IdentityResult.Success, WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)));
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

       

        //public async Task<IActionResult> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
        //{
        //    var user = await _userManager.FindByEmailAsync(request.Email);
        //    if (user == null || user.EmailConfirmed)
        //    {
        //        return Ok(); // Không tiết lộ user có tồn tại hay chưa
        //    }

        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        //    var link = $"https://yourapp.com/confirm-email?userId={user.Id}&token={encodedToken}";

        //    // TODO: Gửi email link này

        //    return Ok();
        //}

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
