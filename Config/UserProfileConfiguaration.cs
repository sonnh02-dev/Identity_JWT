using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityAndJWT.Config
{
    public class UserProfileConfiguaration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {

           builder.ToTable("UserProfiles",Schemas.Patient)
         .HasOne(u => u.IdentityUser)
         .WithOne()
         .HasForeignKey<UserProfile>(u => u.Id)
         .OnDelete(DeleteBehavior.Restrict); // ✅ Không cho cascade
        }
       
    }
}
