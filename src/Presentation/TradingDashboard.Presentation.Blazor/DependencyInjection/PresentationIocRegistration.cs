using Microsoft.Extensions.DependencyInjection;

namespace TradingDashboard.Presentation.Blazor.DependencyInjection;

public static class PresentationIocRegistration
{
    public static IServiceCollection AddPresentationIoc(this IServiceCollection services)
    {
        return services;
    }
}
