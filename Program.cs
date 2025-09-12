using IdentityAndJWT;
using IdentityAndJWT.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



// Add Identity
builder.Services.AddIdentity<IdentityUser<int>, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add CORS (dành cho Postman hoặc frontend khác)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
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

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
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


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(context, services, CancellationToken.None);
}
// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Middleware thứ tự quan trọng!
app.UseHttpsRedirection();

app.UseCors("AllowAll"); // 👈 Thêm nếu gọi từ Postman hoặc FE

app.UseAuthentication(); // 👈 Bắt buộc trước Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
