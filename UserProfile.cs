using Microsoft.AspNetCore.Identity;

namespace IdentityAndJWT
{
    public class UserProfile
    {
        public int Id { get; set; }

        public string FullName { get; set; }
        public IdentityUser<int> IdentityUser { get; set; }
    }
}
