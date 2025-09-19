using Identity_JWT.Application.Abstractions.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = Identity_JWT.Application.Abstractions.Authorization.IAuthorizationService;

namespace Identity_JWT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authorService;

        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorService = authorizationService;
        }

        [HttpGet("check-role")]
        [Authorize]
        public async Task<IActionResult> CheckRole(string role)
        {
            bool inRole = await _authorService.IsInRoleAsync(User, role);
            return Ok(new { InRole = inRole });
        }

        // bạn có thể thêm endpoint kiểm tra policy
        //[HttpGet("check-policy")]
        //[Authorize]
        //public async Task<IActionResult> CheckPolicy(string policyName)
        //{
        //    var result = await _authorService.AuthorizeAsync(User, policyName);
        //    return Ok(new { Authorized = result.Succeeded });
        //}
    }

}
