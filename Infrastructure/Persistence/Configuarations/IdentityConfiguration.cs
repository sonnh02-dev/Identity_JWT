using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Identity;
using Bogus;
using Identity_JWT.Infrastructure.Authentication;

namespace Identity_JWT.Infrastructure.Persistence.Configuarations
{
    public class IdentityConfiguration :
        IEntityTypeConfiguration<UserAccount>,
        IEntityTypeConfiguration<IdentityRole<int>>,
        IEntityTypeConfiguration<IdentityUserRole<int>>,
        IEntityTypeConfiguration<IdentityUserClaim<int>>,
        IEntityTypeConfiguration<IdentityUserLogin<int>>,
        IEntityTypeConfiguration<IdentityRoleClaim<int>>,
        IEntityTypeConfiguration<IdentityUserToken<int>>
    {
        public void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            builder.ToTable("UserAccounts", Schemas.Identity);
        
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
