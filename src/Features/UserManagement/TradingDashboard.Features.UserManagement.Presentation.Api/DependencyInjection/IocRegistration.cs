using FluentValidation;
using TradingDashboard.Application.Common.Notifications;
using TradingDashboard.Application.Common.Security;
using TradingDashboard.Application.Features.Auth.Commands.SignIn;
using TradingDashboard.Infrastructure.Persistence.Implementation.Context;
using TradingDashboard.Presentation.Api.UserManagement.Options;
using TradingDashboard.Presentation.Api.UserManagement.Services;

namespace TradingDashboard.Presentation.Api.UserManagement.DependencyInjection;

public static class IocRegistration
{
    public static IServiceCollection AddIocRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("UserManagementDatabase");
        var skipDbInit = string.Equals(
            Environment.GetEnvironmentVariable("TD_SKIP_DB_INIT"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        

        services.AddDbContext<UserManagementDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure();
            }));

        services.AddIocRegistrationRepository();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SignInCommand).Assembly));
        services.AddValidatorsFromAssemblyContaining<SignInCommandValidator>();

        services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IAuthNotificationService, LoggingAuthNotificationService>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        return services;
    }
}
