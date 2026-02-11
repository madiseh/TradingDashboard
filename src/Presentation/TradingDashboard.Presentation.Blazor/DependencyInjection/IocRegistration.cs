using TradingDashboard.Presentation.Blazor.Features.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace TradingDashboard.Presentation.Blazor.DependencyInjection;

public static class IocRegistration
{
    public static IServiceCollection AddIocRegistration(this IServiceCollection services)
    {
        services.AddHttpClient<ICommonDirectoryClient, CommonDirectoryClient>((sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var baseUrl = configuration["ApiCot:BaseUrl"] ?? "https://localhost:7116";
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}
