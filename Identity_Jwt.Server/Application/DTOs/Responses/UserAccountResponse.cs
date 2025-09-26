namespace Identity_Jwt.Server.Application.DTOs.Responses
{
    public record UserAccountResponse
   (
        int Id,
        string Email,
        string UserName,
        IList<string> Roles
   );
}
