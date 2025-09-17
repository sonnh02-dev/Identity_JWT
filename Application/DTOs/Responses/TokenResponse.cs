namespace Identity_JWT.Application.DTOs.Responses
{
    public sealed class TokenResponse
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Expiration { get; set; }
}
}
