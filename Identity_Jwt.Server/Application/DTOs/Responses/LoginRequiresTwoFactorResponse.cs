namespace Identity_Jwt.Server.Application.DTOs.Responses
{
    public sealed record LoginRequiresTwoFactorResponse(
          string Email,
          bool RequiresTwoFactor = true,
          string? TwoFactorCode = null // chỉ trả ở Dev/Staging
      );
}
