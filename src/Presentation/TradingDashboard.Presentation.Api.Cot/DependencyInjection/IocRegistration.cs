using TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentBySlug;

namespace TradingDashboard.Presentation.Api.Cot.DependencyInjection;

public static class IocRegistration
{
    public static IServiceCollection AddIocRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TradingDashboard")
            ?? "Data Source=db/trading-dashboard.db";

        services.AddDbContext<TradingDashboardDbContext>(options =>
            options.UseSqlite(connectionString));


        services.AddIocRegistrationRepository();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetCotInstrumentBySlugQuery).Assembly));


        return services;
    }
}
