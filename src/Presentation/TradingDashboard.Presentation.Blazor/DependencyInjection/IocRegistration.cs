using Microsoft.Extensions.DependencyInjection;
using TradingDashboard.Presentation.Blazor.Services;
using TradingDashboard.Presentation.Blazor.Services.SiteIndexing;

namespace TradingDashboard.Presentation.Blazor.DependencyInjection;

public static class IocRegistration
{
    public static IServiceCollection AddIocRegistration(this IServiceCollection services)
    {
        services.AddHttpClient(HttpClientNames.ApiCot, (sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var baseUrl = configuration[$"{HttpClientNames.ApiCot}:BaseUrl"] ?? "https://localhost:7116";
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient(HttpClientNames.ApiCurrencyStrength, (sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var baseUrl = configuration[$"{HttpClientNames.ApiCurrencyStrength}:BaseUrl"] ?? "https://localhost:7214";
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient(HttpClientNames.ApiLogin, (sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var baseUrl = configuration[$"{HttpClientNames.ApiLogin}:BaseUrl"] ?? "https://localhost:7181";
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddScoped<AuthSession>();
        services.AddScoped<ISiteIndexingService, SiteIndexingService>();

        return services;
    }
}
