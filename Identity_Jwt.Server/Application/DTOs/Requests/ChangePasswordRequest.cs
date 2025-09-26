using System.ComponentModel.DataAnnotations;

namespace Identity_Jwt.Server.Application.DTOs.Requests
{
    public sealed record ChangePasswordRequest(
        [Required(ErrorMessage = "Old password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Old password must be at least 6 characters")]
        string OldPassword,

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters")]
        string NewPassword
    );
}
