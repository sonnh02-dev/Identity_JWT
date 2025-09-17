using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Identity;
using Bogus;

namespace Identity_JWT.Infrastructure.Persistence.Configuarations
{
    public class IdentityConfiguration :
        IEntityTypeConfiguration<IdentityUser<int>>,
        IEntityTypeConfiguration<IdentityRole<int>>,
        IEntityTypeConfiguration<IdentityUserRole<int>>,
        IEntityTypeConfiguration<IdentityUserClaim<int>>,
        IEntityTypeConfiguration<IdentityUserLogin<int>>,
        IEntityTypeConfiguration<IdentityRoleClaim<int>>,
        IEntityTypeConfiguration<IdentityUserToken<int>>
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

        public void Configure(EntityTypeBuilder<IdentityRole<int>> builder)
        {
            builder.ToTable("Roles", Schemas.Identity);
        }

        public void Configure(EntityTypeBuilder<IdentityUserRole<int>> builder)
        {
            builder.ToTable("UserRoles", Schemas.Identity);
        }

        public void Configure(EntityTypeBuilder<IdentityUserClaim<int>> builder)
        {
            builder.ToTable("UserClaims", Schemas.Identity);
        }

        public void Configure(EntityTypeBuilder<IdentityUserLogin<int>> builder)
        {
            builder.ToTable("UserLogins", Schemas.Identity);
        }

        public void Configure(EntityTypeBuilder<IdentityRoleClaim<int>> builder)
        {
            builder.ToTable("RoleClaims", Schemas.Identity);
        }

        public void Configure(EntityTypeBuilder<IdentityUserToken<int>> builder)
        {
            builder.ToTable("UserTokens", Schemas.Identity);
        }
    }
}
