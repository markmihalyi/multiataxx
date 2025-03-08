using Backend.Data;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
{
    // appsettings.json kialakítása fejlesztői környezettől függően
    if (!Debugger.IsAttached)
    {
        builder.Configuration.AddJsonFile(
            $"Backend/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
            optional: false,
            reloadOnChange: true
         ).AddEnvironmentVariables();
    }

    // Adatbázis beállítása
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // JWT authentikáció beállítása
    string jwtKey = builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Key is missing in configuration.");
    string jwtIssuer = builder.Configuration["JWT:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is missing in configuration.");
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

    builder.Services.AddScoped<AuthService>();
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
}

var app = builder.Build();
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "openapi/v1.json";
    });
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}

app.Run();
