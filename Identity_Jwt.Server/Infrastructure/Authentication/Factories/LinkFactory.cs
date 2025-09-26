using Identity_Jwt.Server.Application.Abstractions.Authentication;

namespace Identity_Jwt.Server.Infrastructure.Authentication.Factories
{
    public class LinkFactory : ILinkFactory
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LinkFactory(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateEmailConfirmationLink(string email, string token)
        {
            return _linkGenerator.GetUriByName(
                _httpContextAccessor.HttpContext!,
                 "ConfirmEmail",        //  Name ở attribute route api
                values: new { email, token }
            )!;
        }

        public string GeneratePasswordResetLink(string email, string token)
        {
            return _linkGenerator.GetUriByName(
                _httpContextAccessor.HttpContext!,
                 "ResetPassword",
                values: new { email, token }
            )!;
        }

        public string GenerateChangeEmailLink(string newEmail, string token)
        {
            return _linkGenerator.GetUriByName(
                _httpContextAccessor.HttpContext!,
                "ChangeEmail",
                values: new { newEmail, token }
            )!;
        }
    }
}
