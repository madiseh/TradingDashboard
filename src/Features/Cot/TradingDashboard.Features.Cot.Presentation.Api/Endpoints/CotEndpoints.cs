using MediatR;
using System.Globalization;
using TradingDashboard.Application.Common.Models;
using TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentBySlug;
using TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentsByGroup;
using TradingDashboard.Application.Features.CotReports.Queries.GetCotLatestReport;
using TradingDashboard.Presentation.Common.Contracts;

namespace TradingDashboard.Presentation.Api.Cot.Endpoints;

public static class CotEndpoints
{
    public static IEndpointRouteBuilder MapCotEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RoutePaths.Api.Cot.CotInstruments)
            .WithTags("CotInstruments");

        group.MapGet("{instrumentSlug}/latest", GetLatestReportAsync)
            .WithName("CotGetLatestReport")
            .WithSummary("Get the latest collected COT report for an instrument")
            .Produces<GetCotLatestReportResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

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

    private static async Task<IResult> GetLatestReportAsync(
        string instrumentSlug,
        string? model,
        string? reportDate,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (!TryParseModel(model, out var parsedModel))
        {
            return TypedResults.BadRequest(new ApiErrorResponse(
                "Validation failed",
                "VALIDATION",
                ["Query parameter 'model' must be 'cot' or 'cot-legacy'."]));
        }

        if (!TryParseReportDate(reportDate, out var parsedReportDate))
        {
            return TypedResults.BadRequest(new ApiErrorResponse(
                "Validation failed",
                "VALIDATION",
                ["Query parameter 'reportDate' must be in 'yyyy-MM-dd' format."]));
        }

        var result = await mediator.Send(
            new GetCotLatestReportQuery(instrumentSlug, parsedModel, parsedReportDate),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors));
        }

        return TypedResults.Ok(result.Data);
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

    private static bool TryParseModel(string? value, out CotReportModel model)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            string.Equals(value, "cot", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, nameof(CotReportModel.Cot), StringComparison.OrdinalIgnoreCase))
        {
            model = CotReportModel.Cot;
            return true;
        }

        if (string.Equals(value, "cot-legacy", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, nameof(CotReportModel.CotLegacy), StringComparison.OrdinalIgnoreCase))
        {
            model = CotReportModel.CotLegacy;
            return true;
        }

        model = CotReportModel.Cot;
        return false;
    }

    private static bool TryParseReportDate(string? value, out DateOnly? reportDate)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            reportDate = null;
            return true;
        }

        if (DateOnly.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedReportDate))
        {
            reportDate = parsedReportDate;
            return true;
        }

        reportDate = null;
        return false;
    }
}
