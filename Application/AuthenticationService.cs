//using Microsoft.AspNetCore.Identity;
//using System.Security.Claims;

//using Microsoft.AspNetCore.WebUtilities;
//using System.Text;
//using EduCore.BackEnd.Infrastructure.Authentication;
//using EduCore.BackEnd.Application.Abstractions.Authentication;

//namespace EduCore.BackEnd.Infrastructure.Authentication
//{
//    public class AuthService : IAuthService
//    {
//        private readonly IMapper mapper;
//        private readonly UserManager<Account> userManager;
//        private readonly SignInManager<Account> signInManager;

//        public AuthService(IMapper mapper, UserManager<Account> UserManager, SignInManager<Account> signInManager)
//        {
//            mapper = mapper;
//            userManager = UserManager;
//            signInManager = signInManager;
//        }

//        public async Task<bool> SignInAsync(SignInDTO request)
//        {
//            SignInResult signInResult = await signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, false);

//            return signInResult.Succeeded;
//        }

//        public async Task SignOutAsync()
//        {
//            await signInManager.SignOutAsync();
//        }

//        public async Task<ApplicationUserDto> GetCurrentUserAsync(ClaimsPrincipal principal)
//        {
//            if (principal == null)
//            {
//                return null;
//            }

//            string userId = userManager.GetUserId(principal);
//            if (string.IsNullOrEmpty(userId))
//            {
//                return null;
//            }

//            Account user = await userManager.FindByIdAsync(userId);

//            if (user == null)
//            {
//                return null;
//            }

//            return mapper.Map<ApplicationUserDto>(user);
//        }

//        public async Task<AuthenticationResponse> SignUpAsync(SignUpRequest request)
//        {
//            Account user = new Account
//            {
//                Email = request.Email,
//                UserName = request.Email
//            };

//            IdentityResult result = await userManager.CreateAsync(user, request.Password);

//            if (result.Succeeded)
//            {
//                await signInManager.SignInAsync(user, isPersistent: false);
//            }

//            return result.ToAuthenticationResult();
//        }

//        public async Task<AuthenticationResponse> ChangePasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword)
//        {
//            Account user = await userManager.GetUserAsync(principal);

//            if (user == null)
//            {
//                return new AuthenticationResponse()
//                {
//                    Succeeded = false,
//                    Errors = new Dictionary<string, string>() { { string.Empty, "Invalid request." } }
//                };
//            }

//            IdentityResult result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

//            return result.ToAuthenticationResult();
//        }

//        public async Task<AuthenticationResponse> ResetPasswordAsync(ResetPasswordRequest request)
//        {
//            Account user = await userManager.FindByEmailAsync(request.UserEmail);

//            if (user == null)
//            {
//                return new AuthenticationResponse()
//                {
//                    Succeeded = false,
//                    Errors = new Dictionary<string, string>() { { string.Empty, "Invalid request." } }
//                };
//            }

//            IdentityResult result = await userManager.ResetPasswordAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token)), request.NewPassword);

//            return result.ToAuthenticationResult();
//        }

//        public async Task<TokenResponse> GeneratePasswordResetTokenAsync(string email)
//        {
//            Account user = await userManager.FindByEmailAsync(email);

//            if (user == null)
//            {
//                return new TokenResponse()
//                {
//                    Succeeded = false,
//                    Errors = new Dictionary<string, string>() { { string.Empty, "Invalid request." } }
//                };
//            }

//            string token = await userManager.GeneratePasswordResetTokenAsync(user);

//            return new TokenResponse()
//            {
//                Succeeded = true,
//                Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token))
//            };
//        }

//        public async Task<TokenResponse> GenerateEmailConfirmationAsync(ClaimsPrincipal principal)
//        {
//            Account user = await userManager.GetUserAsync(principal);

//            if (user == null)
//            {
//                return new TokenResponse()
//                {
//                    Succeeded = false,
//                    Errors = new Dictionary<string, string>() { { string.Empty, "Invalid request." } }
//                };
//            }

//            string code = await userManager.GenerateEmailConfirmationTokenAsync(user);

//            return new TokenResponse()
//            {
//                Succeeded = true,
//                UserId = user.Id,
//                Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code))
//            };
//        }

//        public async Task<AuthenticationResponse> ConfirmEmailAsync(EmailConfirmationRequest request)
//        {
//            Account user = await userManager.FindByIdAsync(request.UserId);

//            if (user == null)
//            {
//                return new AuthenticationResponse()
//                {
//                    Succeeded = false,
//                    Errors = new Dictionary<string, string>() { { string.Empty, "Invalid request." } }
//                };
//            }

//            string token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

//            IdentityResult result = await userManager.ConfirmEmailAsync(user, token);

//            return result.ToAuthenticationResult();
//        }

//        public async Task RefreshSignInAsync(ClaimsPrincipal principal)
//        {
//            Account user = await userManager.GetUserAsync(principal);

//            if (user != null)
//            {
//                await signInManager.RefreshSignInAsync(user);
//            }
//        }

//        public async Task<TokenResponse> GenerateEmailChangeAsync(ClaimsPrincipal principal, string newEmail)
//        {
//            Account user = await userManager.GetUserAsync(principal);

//            if (user == null)
//            {
//                return new TokenResponse()
//                {
//                    Succeeded = false,
//                    Errors = new Dictionary<string, string>() { { string.Empty, "Invalid request." } }
//                };
//            }

//            string token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);

//            return new TokenResponse()
//            {
//                Succeeded = true,
//                Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)),
//                UserId = user.Id
//            };
//        }
//    }
//}