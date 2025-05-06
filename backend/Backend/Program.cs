using AI.Abstractions;
using Backend;
using Backend.Data;
using Backend.GameBase.Serialization;
using Backend.Hubs;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
{
    // Fejlesztői környezettől függően appsettings.json kialakítása 
    if (Debugger.IsAttached)
    {
        builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
    }
    else
    {
        builder.Configuration.AddJsonFile("appsettings.Production.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
    }

    // Adatbázis beállítása
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Token konfiguráció beolvasása, regisztrálása a DI konténerben
    var tokenConfig = new TokenConfig();
    builder.Configuration.GetSection("TokenSettings").Bind(tokenConfig);
    builder.Services.Configure<TokenConfig>(builder.Configuration.GetSection("TokenSettings"));

    // JWT authentikáció
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = tokenConfig.JwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.JwtSecret))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var path = context.HttpContext.Request.Path;

                    var accessToken = context.HttpContext.Request.Cookies["access_token"];

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins", builder =>
        {
            builder.WithOrigins(tokenConfig.CorsFrontendOrigin)
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

    // Services
    builder.Services.AddScoped<AuthService>();
    builder.Services.AddSingleton<ScopedExecutor>();
    builder.Services.AddSingleton<GameService>();

    // Load AI class with loose coupling
    var aiDllPath = Path.Combine(AppContext.BaseDirectory, "AI.dll");
    if (File.Exists(aiDllPath))
    {
        // -> 1. Load AI DLL
        var aiAssembly = Assembly.LoadFrom(aiDllPath);

        try
        {
            // -> 2. Find the IGameAI implementation
            var aiType = aiAssembly.GetTypes().FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IGameAI)));
            if (aiType != null)
            {
                // -> 3. Register to DI container
                builder.Services.AddSingleton(typeof(IGameAI), aiType);
            }
        }
        catch (Exception)
        {
            throw new Exception("The AI.dll does not contain an IGameAI implementation.");
        }
    }

    // Controllers
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new CellStateArrayConverter());
            options.JsonSerializerOptions.Converters.Add(new CellStateArrayListConverter());
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressMapClientErrors = true;
        });

    // OpenAPI + Swagger UI
    builder.Services.AddOpenApi();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "MultiAtaxx", Version = "v1" });
        var xmlFile = "MultiAtaxx.API.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });

    // SignalR
    builder.Services.AddSignalR()
        .AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.Converters.Add(new CellStateArrayConverter());
        });
}

var app = builder.Build();
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MultiAtaxx");
    });

    app.UseCors("AllowSpecificOrigins");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // SignalR route
    app.MapHub<GameHub>("/game");
}

app.Run();
