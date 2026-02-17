using MediatR;
using TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentBySlug;
using TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentsByGroup;
using TradingDashboard.Presentation.Common.Contracts;

namespace TradingDashboard.Presentation.Api.Cot.Endpoints;

public static class CotEndpoints
{
    public static IEndpointRouteBuilder MapCotEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RoutePaths.Api.Cot.CotInstruments)
            .WithTags("CotInstruments");

        group.MapGet("{instrumentSlug}", GetBySlugAsync)
            .WithName("CotGetInstrumentBySlug")
            .WithSummary("Get COT instrument by slug")
            .Produces<GetCotInstrumentBySlugResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("by-group", GetByGroupAsync)
            .WithName("CotGetInstrumentsByGroup")
            .WithSummary("Get COT instruments by group")
            .Produces<IReadOnlyList<GetCotInstrumentsByGroupResponse>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetBySlugAsync(
        string instrumentSlug,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCotInstrumentBySlugQuery(instrumentSlug), cancellationToken);
        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors));
        }

        return TypedResults.Ok(result.Data);
    }

    private static async Task<IResult> GetByGroupAsync(
        string? group,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCotInstrumentsByGroupQuery(group), cancellationToken);
        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors));
        }

        return TypedResults.Ok(result.Data);
    }
}
