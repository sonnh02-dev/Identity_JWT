namespace Identity_JWT.Application.DTOs.Requests
{
    public sealed class ConfirmEmailRequest
    {
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
