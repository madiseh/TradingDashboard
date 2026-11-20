using Microsoft.Extensions.DependencyInjection;
using TradingDashboard.Presentation.Blazor.Services;
using TradingDashboard.Presentation.Blazor.Services.SiteIndexing;

namespace TradingDashboard.Presentation.Blazor.DependencyInjection;

public static class IocRegistration
{
    public static IServiceCollection AddIocRegistration(this IServiceCollection services)
    {
        services.AddHttpClient(HttpClientNames.ApiCot, ConfigureApiClient);
        services.AddHttpClient(HttpClientNames.ApiCurrencyStrength, ConfigureApiClient);
        services.AddHttpClient(HttpClientNames.ApiLogin, ConfigureApiClient);

        services.AddScoped<AuthSession>();
        services.AddScoped<ISiteIndexingService, SiteIndexingService>();
        services.AddScoped<PortalNavigationService>();

        return services;
    }

    private static void ConfigureApiClient(IServiceProvider serviceProvider, HttpClient client)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var baseUrl = configuration["Api:BaseUrl"]
            ?? configuration[$"{HttpClientNames.ApiLogin}:BaseUrl"]
            ?? "https://localhost:7181";

        client.BaseAddress = new Uri(baseUrl);
    }
}
