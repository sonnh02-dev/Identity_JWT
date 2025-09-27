namespace Identity_Jwt.Server.Application.DTOs.Responses
{
    public sealed record LoginSuccessResponse(
          string Email,
          string AccessToken,
          string RefreshToken
      );

}
