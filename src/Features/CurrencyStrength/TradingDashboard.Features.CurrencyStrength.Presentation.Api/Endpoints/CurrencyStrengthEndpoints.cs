using MediatR;
using TradingDashboard.Application.Features.CurrencyStrength.Queries.GetCurrencyStrengthSnapshot;
using TradingDashboard.Application.Features.CurrencyStrength.Queries.GetCurrencyStrengthTimeframes;
using TradingDashboard.Presentation.Common.Contracts;

namespace TradingDashboard.Presentation.Api.CurrencyStrength.Endpoints;

public static class CurrencyStrengthEndpoints
{
    public static IEndpointRouteBuilder MapCurrencyStrengthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RoutePaths.Api.CurrencyStrength.Base)
            .WithTags("CurrencyStrength");

        group.MapGet("timeframes", GetTimeframesAsync)
            .Produces<GetCurrencyStrengthTimeframesResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("timeframes/{timeframeKey}/latest", GetLatestAsync)
            .Produces<GetCurrencyStrengthSnapshotResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetTimeframesAsync(IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCurrencyStrengthTimeframesQuery(), cancellationToken);
        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors));
        }

        return TypedResults.Ok(result.Data);
    }

    private static async Task<IResult> GetLatestAsync(
        string timeframeKey,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCurrencyStrengthSnapshotQuery(timeframeKey), cancellationToken);
        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors));
        }

        return TypedResults.Ok(result.Data);
    }
}
