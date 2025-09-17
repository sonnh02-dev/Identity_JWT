


using EduCore.BackEnd.SharedKernel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EduCore.BackEnd.Infrastructure.Authentication
{
    //public class UserService : IUserService
    //{
    //    public UserManager<Account> _userManager { get; }
    //    public IMapper _mapper { get; }

    //    public UserService(UserManager<Account> userManager, IMapper mapper)
    //    {
    //        _userManager = userManager;
    //        _mapper = mapper;
    //    }

    //    public async Task<AccountResponse> FindByIdAsync(string userId)
    //    {
    //        Account user = await _userManager.FindByIdAsync(userId);

    //        if (user == null)
    //        {
    //            return null;
    //        }

    //        return _mapper.Map<AccountResponse>(user);
    //    }

    //    public async Task<AccountResponse> FindByEmailAsync(string email)
    //    {
    //        Account user = await _userManager.FindByEmailAsync(email);

    //        if (user == null)
    //        {
    //            return null;
    //        }

    //        return _mapper.Map<AccountResponse>(user);
    //    }

    //    public async Task<string> GetUserIdAsync(ClaimsPrincipal principal)
    //    {
    //        Account user = await _userManager.GetUserAsync(principal);
    //        return await _userManager.GetUserIdAsync(user);
    //    }

    //    public async Task<Result> ChangeEmailAsync(ClaimsPrincipal principal, string email, string code)
    //    {
    //        Account user = await _userManager.GetUserAsync(principal);
    //        IdentityResult result = await _userManager.ChangeEmailAsync(user, email, code);
    //        return   result.ToAuthenticationResult();
    //    }

    //    public async Task<bool> IsEmailConfirmedAsync(string email)
    //    {
    //        Account user = await _userManager.FindByEmailAsync(email);

    //        if (user == null)
    //        {
    //            return false;
    //        }

    //        return await _userManager.IsEmailConfirmedAsync(user);
    //    }

    //    public async Task<AuthenticationResponse> ChangePasswordAsync(ClaimsPrincipal principal, string oldPassword, string newPassword)
    //    {
    //        Account user = await _userManager.GetUserAsync(principal);
    //        IdentityResult result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
    //        return result.ToAuthenticationResult();
    //    }

    //    public async Task<bool> HasPasswordAsync(ClaimsPrincipal principal)
    //    {
    //        Account user = await _userManager.GetUserAsync(principal);
    //        return await _userManager.HasPasswordAsync(user);
    //    }

    //    public async Task<string> GetEmailAsync(ClaimsPrincipal principal)
    //    {
    //        Account user = await _userManager.GetUserAsync(principal);
    //        return await _userManager.GetEmailAsync(user);
    //    }

    //    public async Task<string> GetUserNameAsync(ClaimsPrincipal principal)
    //    {
    //        Account user = await _userManager.GetUserAsync(principal);
    //        return await _userManager.GetUserNameAsync(user);
    //    }

    //    public async Task<string> GetPhoneNumberAsync(ClaimsPrincipal principal)
    //    {
    //        Account user = await _userManager.GetUserAsync(principal);
    //        return await _userManager.GetPhoneNumberAsync(user);
    //    }

    //    public async Task<AuthenticationResponse> SetPhoneNumberAsync(ClaimsPrincipal principal, string phoneNumber)
    //    {
    //        Account user = await _userManager.GetUserAsync(principal);
    //        IdentityResult result = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
    //        return result.ToAuthenticationResult();
    //    }

    //    public async Task<AuthenticationResponse> AddPasswordAsync(ClaimsPrincipal principal, string newPassword)
    //    {
    //        Account user = await _userManager.GetUserAsync(principal);
    //        IdentityResult result = await _userManager.AddPasswordAsync(user, newPassword);
    //        return result.ToAuthenticationResult();
    //    }
    //}
}