namespace Identity_Jwt.Server.Infrastructure.Authentication.Settings
{
    public class AuthenticationSettings
    {
        public JwtSettings Jwt { get; set; } = null!;
        public GoogleSettings Google { get; set; } = null!;
        public FacebookSettings Facebook { get; set; } = null!;
    }
}
