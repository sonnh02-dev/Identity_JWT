using Identity_JWT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity_JWT.Infrastructure.Persistence.Configuarations
{
    public class UserProfileConfiguaration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(u => u.UserAccountId)
                    .HasName("PK_UserProfiles_UserAccounts_Id");

            builder.ToTable("UserProfiles", Schemas.Default)
             .HasOne(u => u.UserAccount)
             .WithOne()
             .HasForeignKey<UserProfile>(u => u.UserAccountId)
             .HasConstraintName("FK_UserProfiles_UserAccounts_Id")
             .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
