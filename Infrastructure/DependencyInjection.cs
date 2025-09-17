using Identity_JWT.Application.Abstractions.Email;
using Identity_JWT.Infrastructure.Authentication;
using Identity_JWT.Infrastructure.Email;
using Identity_JWT.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity_JWT.Infrastructure
{
    public static class DependencyInjection
    {
        public static void  AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Add Email 
           services.AddScoped<IEmailService>(sp =>
              new EmailService(
                  smtpHost: "smtp.gmail.com",
                  smtpPort: 587,
                  fromEmail: "youremail@gmail.com",
                  password: "your-app-password", // Gmail cần App Password
                  enableSsl: true
              )
          );
            // Add Identity
            services.AddIdentity<UserAuth, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true; // Bật check email phải unique khi UserManager<UserAuth>.CreateAsync()
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            // Add JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // tắt để test local
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });
            //Add DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                //  .UseAsyncSeeding(async (context, created, ct) =>
                //  {
                //      await DbSeeder.SeedAsync((AppDbContext)context, ct);
                //  })
                //.UseSeeding((context, created) =>
                //{
                //    DbSeeder.SeedAsync((AppDbContext)context, CancellationToken.None).GetAwaiter().GetResult();
                //});

                ;
            });
        }
    }
}
