using TradingDashboard.Features.Cot.CollectService.Abstractions;

namespace TradingDashboard.Features.Cot.CollectService.Endpoints;

public static class CotCollectEndpoints
{
    public static IEndpointRouteBuilder MapCotCollectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/cot-collect")
            .WithTags("CotCollect");

        group.MapGet("/status", GetStatusAsync)
            .WithName("CotCollectStatus")
            .WithSummary("Get collector status and last fetch timestamps");

        group.MapPost("/run", RunAsync)
            .WithName("CotCollectRun")
            .WithSummary("Run the collector immediately");

        return app;
    }

    private static async Task<Ok<CotCollectionStatusResponse>> GetStatusAsync(
        ICotCollectionCoordinator coordinator,
        CancellationToken cancellationToken)
    {
        var status = await coordinator.GetStatusAsync(cancellationToken);
        return TypedResults.Ok(status);
    }

    private static async Task<Results<Ok<CotCollectionStatusResponse>, Conflict<string>>> RunAsync(
        ICotCollectionCoordinator coordinator,
        CancellationToken cancellationToken)
    {
        try
        {
            var status = await coordinator.RunOnceAsync("manual", cancellationToken);
            return TypedResults.Ok(status);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
    }
}
