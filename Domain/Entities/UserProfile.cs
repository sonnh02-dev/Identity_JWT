using Microsoft.AspNetCore.Identity;

namespace Identity_JWT.Domain.Entities
{
    public class UserProfile
    {
        public int UserAccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string UrlImg { get; set; } = null!;
        public IdentityUser<int> UserAccount { get; set; } = null!;
    }
}
