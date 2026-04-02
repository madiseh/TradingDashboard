namespace TradingDashboard.Presentation.MauiHybrid.DependencyInjection;

public static class IocRegistration
{
    public static IServiceCollection AddIocRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLocalization();
        services.AddScoped(_ => new Services.ApiHttpClientFactory(configuration));
        services.AddScoped<Services.AuthSession>();

        return services;
    }
}
