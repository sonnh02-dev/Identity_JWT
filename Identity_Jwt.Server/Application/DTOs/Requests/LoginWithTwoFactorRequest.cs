using Azure.Core;
using System.ComponentModel.DataAnnotations;

namespace Identity_Jwt.Server.Application.DTOs.Requests
{
    public sealed record LoginWithTwoFactorRequest(

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        string Email,

        [Required(ErrorMessage = "Two factor code is required")]
        string TwoFactorCode
    );
}
