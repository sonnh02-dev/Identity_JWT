using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Identity_JWT.Application.DTOs.Requests
{
    public class SignInRequest

    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        public bool IsRememberMe { get; set; } = false;
    }
}
