using Identity_JWT.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.Data;
using Identity_JWT.Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Identity_JWT.Application.Abstractions.Email;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Identity_JWT.API.Extensions;
namespace Identity_JWT.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenService;
        private readonly IEmailService _emailService;

        public AuthenticationController(IAuthenticationService authService, IEmailService emailService)
        {
            _authenService = authService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authenService.RegisterAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Errors);

        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
        {
            var result = await _authenService.ConfirmEmailAsync(request);
            return result.IsSuccess ? Ok("Email confirmed successfully!") : BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authenService.LoginAsync(request);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails(this);


        }
        [HttpPost("request-password-reset")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authenService.RequestPasswordResetAsync(request.Email);

            return Ok("Nếu email tồn tại trong hệ thống, hướng dẫn reset đã được gửi.");
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authenService.ResetPasswordAsync(request);
            return result.IsSuccess ? Ok("Password has been reset successfully.") : result.ToProblemDetails(this);
        }

        [HttpPost("request-change-email/{newEmail}")]
        [Authorize]
        public async Task<IActionResult> RequestChangeEmail(string newEmail)
        {

            var result = await _authenService.RequestChangeEmailAsync(User, newEmail);
            return result.IsSuccess ? Ok("Confirmation email sent.") : BadRequest(result.Errors);
        }

        [HttpGet("confirm-change-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmChangeEmail([FromQuery] int userId, [FromQuery] string email, [FromQuery] string token)
        {
            var result = await _authenService.ConfirmChangeEmailAsync(userId, email, token);

            return result.IsSuccess ? Ok("Email changed successfully.") : BadRequest(result.Errors);
        }



    }

}
