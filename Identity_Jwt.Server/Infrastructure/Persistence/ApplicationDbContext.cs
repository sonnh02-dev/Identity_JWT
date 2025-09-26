using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using Identity_Jwt.Server.Domain.Entities;
using Identity_Jwt.Server.Infrastructure.Persistence.Configuarations;
using Identity_Jwt.Server.Infrastructure.Authentication.Entities;

namespace Identity_Jwt.Server.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<UserAccount, IdentityRole<int>,int,
    IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<UserProfile> UserProfiles { get; set; }

   // Identity sẽ tự quản lý Users/Roles thông qua UserManager, RoleManager, SignInManager ko cần DbSet
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);  
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);


    }


}
