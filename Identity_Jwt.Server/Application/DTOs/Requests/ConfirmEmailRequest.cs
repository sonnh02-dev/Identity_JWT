using System.ComponentModel.DataAnnotations;

namespace Identity_Jwt.Server.Application.DTOs.Requests
{
    public sealed class ConfirmEmailRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = null!;
    }
}
