namespace Identity_Jwt.Server.Application.DTOs.Responses
{
    public sealed record LoginResponse(
     string Email,
     string? AccessToken = null,
     string? RefreshToken = null,
     bool RequiresTwoFactor = false,
     int FailedLoginAttempts = 0
   );

}
