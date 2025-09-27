using Identity_Jwt.Server.Infrastructure.Email;
using Identity_Jwt.Server.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Mail;
using System.Net;
using Identity_Jwt.Server.Infrastructure.Authorization;
using Identity_Jwt.Server.Infrastructure.Authentication.Entities;
using Identity_Jwt.Server.Infrastructure.Authentication.Factories;
using Identity_Jwt.Server.Infrastructure.Authentication.Services;
using Identity_Jwt.Server.Infrastructure.Authentication.Settings;
using Identity_Jwt.Server.Application.Abstractions.Authorization;
using Identity_Jwt.Server.Application.Abstractions.Authentication;
using Identity_Jwt.Server.Domain.IRepositories;
using StackExchange.Redis;
using Identity_Jwt.Server.Application.Abstractions.Email;

namespace Identity_Jwt.Server.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Add config fluent email
            var _smtpSettings = configuration.GetSection("SmtpEmail").Get<SmtpEmailSettings>();
            services.AddFluentEmail(_smtpSettings?.FromEmail, _smtpSettings?.DisplayName)
                    .AddRazorRenderer()
                    .AddSmtpSender(() =>
                       new SmtpClient(_smtpSettings?.SmtpHost, _smtpSettings.SmtpPort)
                       {
                           Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                           EnableSsl = _smtpSettings.EnableSsl,
                           Timeout = 20000 
                       });
            services.AddScoped<IEmailSender, EmailSender>();


            // Add identity
            services.AddIdentity<UserAccount, IdentityRole<int>>(options =>
            {
                // --- User settings ---
                options.User.RequireUniqueEmail = true; // Bật check email phải unique khi UserManager<UserAuth>.CreateAsync()

                // --- Lockout settings ---
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            // Add config  authentication
            services.Configure<JwtSettings>(configuration.GetSection("Authentication:Jwt"));

            var _jwtSettings = configuration.GetSection("Authentication:Jwt").Get<JwtSettings>();

            services.AddAuthentication(options =>
                  {
                      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                  })
                  .AddJwtBearer(options =>
                  {
                      options.RequireHttpsMetadata = false;
                      options.SaveToken = true;
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          ValidateLifetime = true, //Check expired
                          ValidateIssuerSigningKey = true,
                          ValidIssuer = _jwtSettings?.Issuer,
                          ValidAudience = _jwtSettings?.Audience,
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings?.Key!))
                      };
                      //Client có thể  kiểm tra header Token-Expired để biết token hết hạn
                      options.Events = new JwtBearerEvents
                      {
                          OnAuthenticationFailed = context =>
                          {
                              if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                              {
                                  context.Response.Headers.Append("Token-Expired", "true");
                              }
                              return Task.CompletedTask;
                          }
                      };
                  })
                  .AddGoogle(options =>
                   {
                       var _ggSettings = configuration.GetSection("Authentication:Google").Get<GoogleSettings>();
                       options.ClientId = _ggSettings?.ClientId!;
                       options.ClientSecret = _ggSettings?.ClientSecret!;
                   })
                  .AddFacebook(options =>
                    {
                        var _fbSettings = configuration.GetSection("Authentication:Facebook").Get<FacebookSettings>();
                        options.AppId = _fbSettings?.AppId!;
                        options.AppSecret = _fbSettings?.AppSecret!;
                    }); ;


            //Add dbContext
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
            // Redis connection
            var redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!);
            services.AddSingleton<IConnectionMultiplexer>(redis);


            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IRefreshTokenRepository, RedisRefreshTokenRepository>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();

            services.AddScoped<ILinkFactory, LinkFactory>();
        }

    }
}
