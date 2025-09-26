using Identity_Jwt.Server.Application.DTOs.Requests;
using Identity_Jwt.Server.Application.DTOs.Responses;
using Identity_Jwt.Server.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Jwt.Server.Presentation.Controllers
{
    [ApiController]
    [Route("api/user-accounts")]
    [Authorize] // All endpoints require authentication by default
    public class UserAccountsController : ControllerBase
    {
        private readonly IUserAccountService _userAccountService;

        public UserAccountsController(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        // -------------------------------------- Profile ----------------------------

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userAccountService.GetCurrentUserAsync<UserAccountResponse>(User);
            return Ok(user);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userAccountService.GetUserByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails(this);
        }
        [AllowAnonymous]
        [HttpGet("by-email")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var result = await _userAccountService.GetUserByEmailAsync(email);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails(this);
        }

        // -------------------------------------- Password ------------------------------

        [HttpPost("add-password")]
        public async Task<IActionResult> AddPassword([FromBody] string newPassword)
        {
            var result = await _userAccountService.AddPasswordAsync(User, newPassword);
            return result.IsSuccess ? Ok() : result.ToProblemDetails(this);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _userAccountService.ChangePasswordAsync(User, request.OldPassword, request.NewPassword);
            return result.IsSuccess ? Ok() : result.ToProblemDetails(this);
        }

        // ----------------------------------- Email ---------------------------------

        [HttpPost("request-change-email/{newEmail}")]
        public async Task<IActionResult> RequestChangeEmail(string newEmail)
        {
            var result = await _userAccountService.RequestChangeEmailAsync(User, newEmail);
            return result.IsSuccess ? Ok() : result.ToProblemDetails(this);
        }

        [AllowAnonymous]
        [HttpPost("change-email", Name = "ChangeEmail")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
        {
            var result = await _userAccountService.ChangeEmailAsync(User, request);
            return result.IsSuccess ? Ok() : result.ToProblemDetails(this);
        }

        // ------------------------------------- Phone --------------------------------

        [HttpPost("request-change-phone-number/{newPhoneNumber}")]
        public async Task<IActionResult> RequestChangePhone(string newPhoneNumber)
        {
            var result = await _userAccountService.RequestChangePhoneNumberAsync(User, newPhoneNumber);
            return result.IsSuccess ? Ok() : result.ToProblemDetails(this);
        }

        [HttpPost("change-phone-number")]
        public async Task<IActionResult> ChangePhoneNumber([FromBody] ChangePhoneNumberRequest request)
        {
            var result = await _userAccountService.ChangePhoneNumberAsync(User, request);
            return result.IsSuccess ? Ok() : result.ToProblemDetails(this);
        }

        [HttpPost("set-phone")]
        public async Task<IActionResult> SetPhone([FromBody] string phoneNumber)
        {
            var result = await _userAccountService.SetPhoneNumberAsync(User, phoneNumber);
            return result.IsSuccess ? Ok() : result.ToProblemDetails(this);
        }
        //---------------------------------------------------------------------
        [HttpPost("enable-two-factor")]
        public async Task<IActionResult> EnableTwoFactor()
        {
            var result = await _userAccountService.EnableTwoFactorAsync(User);

            return result.IsSuccess ? Ok() : result.ToProblemDetails(this);

        }

    }


}
