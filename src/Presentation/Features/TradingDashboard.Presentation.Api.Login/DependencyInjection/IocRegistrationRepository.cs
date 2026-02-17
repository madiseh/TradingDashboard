using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Infrastructure.Persistence.Implementation.Context;
using TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

namespace TradingDashboard.Presentation.Api.Login.DependencyInjection;

public static class IocRegistrationRepository
{
    public static IServiceCollection AddIocRegistrationRepository(this IServiceCollection services)
    {
        services.AddScoped<DbContext, LoginDbContext>();

        services.AddScoped<IAppUserQueryRepository, AppUserQueryRepository>();
        services.AddScoped<IAppUserCommandRepository, AppUserCommandRepository>();

        return services;
    }
}
