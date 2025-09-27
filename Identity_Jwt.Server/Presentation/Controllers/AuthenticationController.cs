using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.Data;
using Identity_Jwt.Server.Application.DTOs.Requests;
using Identity_Jwt.Server.Presentation.Extensions;
using Identity_Jwt.Server.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Authorization;
namespace Identity_Jwt.Server.Presentation.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenService;
        private readonly ITokenService _tokenService;

        public AuthenticationController(IAuthenticationService authService, ITokenService tokenService)
        {
            _authenService = authService;
            _tokenService = tokenService;
        }

        //---------------------------------------- Register & Confirm Email -------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authenService.RegisterAsync(request);
            return result.IsSuccess ? Ok(result) : result.ToProblemDetails(this);

        }
        [HttpGet("confirm-email", Name = "ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
        {
            var result = await _authenService.ConfirmEmailAsync(request);
            return result.IsSuccess ? Ok("Email confirmed successfully!") : result.ToProblemDetails(this);
        }
        //-------------------------------------------- Login & Logout ---------------------------------
        [AllowAnonymous]
        [HttpPost("login-with-password")]
        public async Task<IActionResult> LoginWithPassword([FromBody] LoginRequest request)
        {
            var result = await _authenService.LoginWithPasswordAsync(request);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails(this);
        }
        [AllowAnonymous]
        [HttpPost("login-with-two-factor")]
        public async Task<IActionResult> LoginWithTwoFactor([FromBody] LoginWithTwoFactorRequest request)
        {
            var result = await _authenService.LoginWithTwoFactorAsync(request);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails(this);
        }
        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromForm] string provider, [FromQuery] string returnUrl = null)
        {
            var properties = await _authenService.GetExternalLoginPropertiesAsync(provider, returnUrl ?? "/");
            return Challenge(properties, provider);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback([FromQuery] string returnUrl = null)
        {
            var result = await _authenService.ExternalLoginCallbackAsync();
            return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails(this);

        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authenService.LogoutAsync(User);
            return Ok();
        }


        //----------------------------------------------Reset Password -------------------------------------

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authenService.RequestPasswordResetAsync(request.Email);

            return Ok("Nếu email tồn tại trong hệ thống, hướng dẫn reset đã được gửi.");
        }



        [HttpPost("reset-password", Name = "ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authenService.ResetPasswordAsync(request);
            return result.IsSuccess ? Ok("Password has been reset successfully.") : result.ToProblemDetails(this);
        }

        //------------------------------------------- Refresh Access Token ---------------------------------

        [HttpPost("refresh-access-token")]
        public async Task<IActionResult> RefreshAccessToken([FromBody] string refreshToken)
        {
            var result = await _tokenService.RefreshAccessTokenAsync(refreshToken);
            return result.IsSuccess ? Ok(result) : result.ToProblemDetails(this);

        }

    }

}
