using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Identity;
using Bogus;

namespace IdentityAndJWT.Config
{
    public class UserAccountConfiguration : IEntityTypeConfiguration<IdentityUser<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityUser<int>> builder)
        {
            builder.ToTable("UserAccounts", Schemas.Identity);

            var passwordHasher = new PasswordHasher<IdentityUser<int>>();

            var faker = new Faker("en");
            var users = new List<IdentityUser<int>>();

            for (int i = 1; i <= 5; i++)
            {
                var user = new IdentityUser<int>
                {
                    Id = i,
                    UserName = faker.Internet.UserName(),
                    NormalizedUserName = faker.Internet.UserName().ToUpper(),
                    Email = faker.Internet.Email(),
                    NormalizedEmail = faker.Internet.Email().ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = faker.Phone.PhoneNumber("###-###-####"),
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "P@ssword123");

                users.Add(user);
            }

            builder.HasData(users);
        }
    }
}
