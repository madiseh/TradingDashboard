using TradingDashboard.Presentation.Blazor.Features.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace TradingDashboard.Presentation.Blazor.DependencyInjection;

public static class PresentationIocRegistration
{
    public static IServiceCollection AddPresentationIoc(this IServiceCollection services)
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
