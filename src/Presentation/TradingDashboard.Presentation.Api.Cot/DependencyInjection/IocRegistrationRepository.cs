using TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

namespace TradingDashboard.Presentation.Api.Cot.DependencyInjection;

public static class IocRegistrationRepository
{
    public static IServiceCollection AddIocRegistrationRepository(this IServiceCollection services)
    {
        services.AddScoped<DbContext, TradingDashboardDbContext>();

        services.AddScoped<ICotInstrumentQueryRepository, CotInstrumentRepository>();
        return services;
    }
}
