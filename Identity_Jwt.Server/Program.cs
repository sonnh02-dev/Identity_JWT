using Identity_Jwt.Server.Infrastructure;
using Identity_Jwt.Server.Infrastructure.Persistence;
using Identity_Jwt.Server.Presentation.Infrastructure;

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

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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
app.UseAuthorization();   //[Middleware JwtBearer]
                          //Lấy token từ Authorization header
                          // Validate:Signature,Issuer,Audience,Expiration                           
                          //    ├─- Nếu fail -> 401 Unauthorized
                          //    └─- Nếu pass -> Tạo ClaimsPrincipal gắn vào HttpContext.User

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
