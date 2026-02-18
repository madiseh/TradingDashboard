using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TradingDashboard.Application.Common.Security;
using TradingDashboard.Infrastructure.Persistence.Implementation.Seed;
using TradingDashboard.Presentation.Api.Login.DependencyInjection;
using TradingDashboard.Presentation.Api.Login.Endpoints;
using TradingDashboard.Presentation.Api.Login.Options;

var builder = Program.CreateHostBuilder(args).Build();
var app = Program.ConfigureApp(builder);
app.Run();

public partial class Program
{
    public static WebApplicationBuilder CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ApplicationName = typeof(Program).Assembly.FullName,
            ContentRootPath = AppContext.BaseDirectory
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument(options =>
        {
            options.DocumentName = "v1";
            options.Title = "Trading Dashboard API - Login";
            options.Version = "v1";
        });
        builder.Services.AddHealthChecks();

        builder.Services.AddIocRegistration(builder.Configuration);

        var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var keyBytes = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();
        return builder;
    }

    public static WebApplication ConfigureApp(WebApplication app)
    {
        var skipDbInit = string.Equals(
            Environment.GetEnvironmentVariable("TD_SKIP_DB_INIT"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (!skipDbInit)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LoginDbContext>();
            var passwordHashingService = scope.ServiceProvider.GetRequiredService<IPasswordHashingService>();
           

            dbContext.Database.Migrate();
            AdminUserSeedBootstrapper.SeedMissingAdmin(dbContext, passwordHashingService);
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks(RoutePaths.ApiCommon.Health);
        app.MapAuthEndpoints();

        return app;
    }
}
