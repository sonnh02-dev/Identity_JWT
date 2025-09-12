
using System.ComponentModel.DataAnnotations;

namespace IdentityAndJWT
{
    // Models/RegisterModel.cs
    public class RegisterModel
    {
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Models/LoginModel.cs
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

}
