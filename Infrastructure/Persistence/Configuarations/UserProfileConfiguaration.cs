using Identity_JWT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity_JWT.Infrastructure.Persistence.Configuarations
{
    public class UserProfileConfiguaration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {

         builder.ToTable("UserProfiles",Schemas.Default)
         .HasOne(u => u.UserAccount)
         .WithOne()
         .HasForeignKey<UserProfile>(u => u.UserAccountId)
         .OnDelete(DeleteBehavior.Restrict); 
        }
       
    }
}
