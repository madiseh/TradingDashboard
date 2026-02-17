using FluentValidation;
using TradingDashboard.Application.Common.Security;
using TradingDashboard.Application.Features.Auth.Commands.SignIn;
using TradingDashboard.Infrastructure.Persistence.Implementation.Context;
using TradingDashboard.Presentation.Api.Login.Options;
using TradingDashboard.Presentation.Api.Login.Services;

namespace TradingDashboard.Presentation.Api.Login.DependencyInjection;

public static class IocRegistration
{
    public static IServiceCollection AddIocRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LoginDatabase");
        var skipDbInit = string.Equals(
            Environment.GetEnvironmentVariable("TD_SKIP_DB_INIT"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            if (skipDbInit)
            {
                connectionString = "Server=(localdb)\\mssqllocaldb;Database=TradingDashboard_Login_Swagger;Trusted_Connection=True;TrustServerCertificate=True;";
            }
            else
            {
                throw new InvalidOperationException("Connection string 'LoginDatabase' is required.");
            }
        }

        services.AddDbContext<LoginDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure();
            }));

        services.AddIocRegistrationRepository();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SignInCommand).Assembly));
        services.AddValidatorsFromAssemblyContaining<SignInCommandValidator>();

        services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        return services;
    }
}
