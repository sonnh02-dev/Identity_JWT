namespace Identity_JWT.Application.DTOs.Requests
{
    public sealed class ConfirmEmailRequest
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
