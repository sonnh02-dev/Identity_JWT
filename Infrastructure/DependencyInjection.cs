using Identity_Jwt.Infrastructure.Email;
using Identity_Jwt.Infrastructure.Email.Ses;
using Identity_Jwt.Infrastructure.Email.Smtp;
using Identity_JWT.Application.Abstractions.Authentication;
using Identity_JWT.Application.Abstractions.Authorization;
using Identity_JWT.Application.Abstractions.Email;
using Identity_JWT.Infrastructure.Authentication;
using Identity_JWT.Infrastructure.Authorization;
using Identity_JWT.Infrastructure.Email;
using Identity_JWT.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity_JWT.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Add Email 
            services.Configure<SesEmailSettings>(configuration.GetSection("SesEmailSettings"));
            services.Configure<SmtpEmailSettings>(configuration.GetSection("SmtpEmailSettings"));
            services.AddTransient<IEmailService, SesEmailService>();
            services.AddTransient<EmailMessageFactory>();



            // Add Identity
            services.AddIdentity<UserAccount, IdentityRole<int>>(options =>
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
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
        }
    }
}
