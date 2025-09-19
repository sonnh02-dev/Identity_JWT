//// Controllers/AuthController.cs
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//namespace IdentityAndJWT.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class AuthController : ControllerBase
//{
//    private readonly UserManager<IdentityUser<int>> _userManager;
//    private readonly IConfiguration _configuration;
//    private readonly AppDbContext _context;

//    public AuthController(UserManager<IdentityUser<int>> userManager, IConfiguration configuration,AppDbContext context)
//    {
//        _userManager = userManager;
//        _configuration = configuration;
//        _context =context;
//    }
//    [HttpGet("long-operation")]
//    public async Task<IActionResult> LongOperation(CancellationToken cancellationToken)
//    {
//        try
//        {
//            for (int i = 0; i < 10; i++)
//            {
//                //cancellationToken.ThrowIfCancellationRequested();
//                Console.WriteLine($"Processing step {i + 1}");
//                await Task.Delay(1000, cancellationToken);
//            }

//            return Ok("Hoàn tất");
//        }
//        catch (OperationCanceledException)
//        {
//            Console.WriteLine("Yêu cầu đã bị hủy bởi client.");
//            return StatusCode(499, "Client đã hủy yêu cầu.");
//        }
//    }


//    [HttpGet("all")]
//    public async Task<IActionResult> Get()
//    {
//        return Ok(await _context.UserProfiles.Select(p => new { Id = p.Id ,Fullname = p.FullName }).ToListAsync() );
//    }

//    [HttpPost("register")]
//    public async Task<IActionResult> Register([FromBody] RegisterModel model)
//    {
//        var user = new IdentityUser<int> { UserName = model.UserName, Email = model.Email };
//        var result = await _userManager.CreateAsync(user, model.Password);

//        if (result.Succeeded)
//            return Ok("User created successfully");

//        return BadRequest(result.Errors);
//    }

//    [HttpPost("login")]
//    public async Task<IActionResult> Login([FromBody] LoginModel model)
//    {
//        var user = await _userManager.FindByEmailAsync(model.Email);
//        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
//        {
//            var authClaims = new[]
//            {
//                new Claim(ClaimTypes.Name, user.UserName),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//            };

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
//            var token = new JwtSecurityToken(
//                issuer: _configuration["Jwt:Issuer"],
//                audience: _configuration["Jwt:Audience"],
//                expires: DateTime.Now.AddHours(1),
//                claims: authClaims,
//                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
//            );

//            return Ok(new
//            {
//                token = new JwtSecurityTokenHandler().WriteToken(token),
//                expiration = token.ValidTo
//            });
//        }

//        return Unauthorized();
//    }

//    [HttpGet("profile")]
//    [Authorize]
//    public IActionResult GetProfile()
//    {
//        var name = User.Identity?.Name;

//        // Lấy danh sách tất cả claim
//        var claims = User.Claims.Select(c => new { c.Type, c.Value });

//        // Lấy role (nếu có)
//        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

//        // Lấy permission (nếu có)
//        var permission = User.Claims.FirstOrDefault(c => c.Type == "permission")?.Value;

//        return Ok(new
//        {
//            Name = name,
//            Role = role,
//            Permission = permission,
//            AllClaims = claims
//        });
//    }

//}

