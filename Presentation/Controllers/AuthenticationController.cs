using Identity_JWT.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.Data;
using Identity_JWT.Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using EduCore.BackEnd.Infrastructure.Authentication;
using Identity_JWT.Application.Abstractions.Email;
using Azure.Core;
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
            return result.Succeeded ? Ok(result) : BadRequest(result.Errors);

        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
        {
            var result = await _authenService.ConfirmEmailAsync(request);
            return result.Succeeded ? Ok("Email confirmed successfully!")  : BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authenService.LoginAsync(request);
            return result.Succeeded ? Ok(result) : result.to;

        }
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authenService.ForgotPasswordAsync(request.Email);

            return Ok("Nếu email tồn tại trong hệ thống, hướng dẫn reset đã được gửi.");
        }



        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authenService.ResetPasswordAsync(request);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok("Password has been reset successfully.");
        }

        // nếu bạn có login, refresh token thì đặt chung ở đây luôn
        //[HttpPost("login")]
        //[HttpPost("refresh-token")]



    }

}
