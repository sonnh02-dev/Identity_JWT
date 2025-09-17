using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using Identity_JWT.Domain.Entities;
using Identity_JWT.Infrastructure.Persistence.Configuarations;

namespace Identity_JWT.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>,int,
    IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<IdentityRole<int>> Roles { get; set; }
    public DbSet<IdentityUser<int>> UserAccounts { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole<int>>().ToTable("Roles",Schemas.Identity);
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles", Schemas.Identity);
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims", Schemas.Identity);
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins", Schemas.Identity);
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims", Schemas.Identity);
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens", Schemas.Identity);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);


    }


}
