using Identity_JWT.Infrastructure;
using Identity_JWT.Infrastructure.Authentication;
using Identity_JWT.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);






// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    await DbSeeder.SeedAsync(context, services, CancellationToken.None);
}
// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Middleware thứ tự quan trọng!
app.UseHttpsRedirection();

app.UseCors("AllowAll"); 

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
