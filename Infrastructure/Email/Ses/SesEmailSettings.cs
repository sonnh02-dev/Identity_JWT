namespace Identity_Jwt.Infrastructure.Email.Ses
{
    public class SesEmailSettings
    {
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string Region { get; set; } = null!; 
        public string FromEmail { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }

}
