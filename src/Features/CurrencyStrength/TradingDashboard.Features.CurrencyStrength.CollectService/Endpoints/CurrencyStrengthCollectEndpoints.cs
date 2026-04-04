using TradingDashboard.Features.CurrencyStrength.CollectService.Abstractions;
using TradingDashboard.Features.CurrencyStrength.CollectService.Models;

namespace TradingDashboard.Features.CurrencyStrength.CollectService.Endpoints;

public static class CurrencyStrengthCollectEndpoints
{
    public static IEndpointRouteBuilder MapCurrencyStrengthCollectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/collector/currency-strength");

        group.MapGet("status", (ICurrencyStrengthCollectionCoordinator coordinator, CancellationToken cancellationToken)
            => coordinator.GetStatusAsync(cancellationToken))
            .Produces<CurrencyStrengthCollectionStatusResponse>(StatusCodes.Status200OK);

        group.MapPost("run-once", (ICurrencyStrengthCollectionCoordinator coordinator, CancellationToken cancellationToken)
            => coordinator.RunOnceAsync("manual", cancellationToken))
            .Produces<CurrencyStrengthCollectionStatusResponse>(StatusCodes.Status200OK);

        return app;
    }
}
