using System.ComponentModel.DataAnnotations;

namespace Identity_Jwt.Server.Application.DTOs.Requests
{
    public sealed record ChangePhoneNumberRequest(
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        string NewPhoneNumber,

        [Required(ErrorMessage = "Token is required")]
        string Token
    );
}
