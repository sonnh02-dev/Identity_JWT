using Bogus;
using Identity_JWT.Domain.Entities;
using Identity_JWT.Infrastructure.Authentication;
using Identity_JWT.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, IServiceProvider services, CancellationToken ct)
    {
        var userManager = services.GetRequiredService<UserManager<UserAccount>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        // 1. Seed Roles
        var roles = new List<string> { "Admin", "User", "Manager" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }

        // 2. Seed RoleClaims
        var adminRole = await roleManager.FindByNameAsync("Admin");
        if (adminRole != null)
        {
            var claims = await roleManager.GetClaimsAsync(adminRole);
            if (!claims.Any(c => c.Type == "Permission" && c.Value == "User.Manage"))
            {
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", "User.Manage"));
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", "Role.Manage"));
            }
        }

        // 3. Seed Users
        if (!await context.Users.AnyAsync(ct))
        {
            var faker = new Faker("vi");

            // Tạo Admin user
            var adminUser = new UserAccount
            {
                UserName = "admin",
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "P@ssword123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                await userManager.AddClaimAsync(adminUser, new Claim("Permission", "System.FullAccess"));

                // Seed UserProfile cho Admin
                context.UserProfiles.Add(new UserProfile
                {
                    UserAccountId = adminUser.Id,
                    FullName = "System Administrator",
                    Address = "Hà Nội",
                    UrlImg = "https://example.com/avatar/admin.png"
                });
            }

            // Seed thêm vài User random
            for (int i = 1; i <= 5; i++)
            {
                var user = new UserAccount
                {
                    UserName = faker.Internet.UserName(),
                    Email = faker.Internet.Email(),
                    EmailConfirmed = true
                };

                var userResult = await userManager.CreateAsync(user, "P@ssword123");
                if (userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");

                    context.UserProfiles.Add(new UserProfile
                    {
                        UserAccountId = user.Id,
                        FullName = faker.Name.FullName(),
                        Address = faker.Address.FullAddress(),
                        UrlImg = faker.Internet.Avatar()
                    });
                }
            }

            await context.SaveChangesAsync(ct);
        }
    }
}
