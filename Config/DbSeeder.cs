// DbSeeder.cs
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IdentityAndJWT;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IServiceProvider services, CancellationToken ct)
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser<int>>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        if (!await context.UserProfiles.AnyAsync(ct))
        {
            var faker = new Faker("vi");

            for (int i = 1; i <= 10; i++)
            {
                var email = $"user{i}@example.com";
                var user = new IdentityUser<int>
                {
                    UserName = $"user{i}",
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "P@ssword123");

                if (result.Succeeded)
                {
                    var profile = new UserProfile
                    {
                        Id = user.Id,
                        FullName = faker.Name.FullName()
                    };

                    context.UserProfiles.Add(profile);
                }
            }

            await context.SaveChangesAsync(ct);
        }
    }
}
