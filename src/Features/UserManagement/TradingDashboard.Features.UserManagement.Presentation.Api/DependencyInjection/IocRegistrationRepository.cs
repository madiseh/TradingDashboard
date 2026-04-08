using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Application.Repository.Subscriptions;
using TradingDashboard.Infrastructure.Persistence.Implementation.Context;
using TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

namespace TradingDashboard.Presentation.Api.UserManagement.DependencyInjection;

public static class IocRegistrationRepository
{
    public static IServiceCollection AddIocRegistrationRepository(this IServiceCollection services)
    {
        services.AddScoped<DbContext, UserManagementDbContext>();

        services.AddScoped<IAppUserQueryRepository, AppUserQueryRepository>();
        services.AddScoped<IAppUserCommandRepository, AppUserCommandRepository>();
        services.AddScoped<ISubscriptionQueryRepository, SubscriptionQueryRepository>();
        services.AddScoped<ISubscriptionCommandRepository, SubscriptionCommandRepository>();

        return services;
    }
}
