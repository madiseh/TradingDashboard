using TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;
using TradingDashboard.Application.Repository.Cot;

namespace TradingDashboard.Presentation.Api.Cot.DependencyInjection;

public static class IocRegistrationRepository
{
    public static IServiceCollection AddIocRegistrationRepository(this IServiceCollection services)
    {
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<CotDbContext>());

        services.AddScoped<ICotInstrumentQueryRepository, CotInstrumentQueryRepository>();
        return services;
    }
}
