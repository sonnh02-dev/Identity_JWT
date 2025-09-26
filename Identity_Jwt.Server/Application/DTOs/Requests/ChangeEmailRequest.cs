using Azure.Core;
using System.ComponentModel.DataAnnotations;

namespace Identity_Jwt.Server.Application.DTOs.Requests
{
    public sealed record ChangeEmailRequest(
        int UserId,

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        string NewEmail,

        [Required(ErrorMessage = "Token is required")]
        string Token
    );
}
