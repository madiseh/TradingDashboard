using FluentValidation;
using MediatR;
using System.Globalization;
using TradingDashboard.Application.Common.Models;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Repository.Cot;

namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotReportSeries;

public sealed class GetCotReportSeriesHandler(
    ICotInstrumentQueryRepository instrumentRepository,
    ICotHistoricalReportQueryRepository historicalReportRepository,
    IValidator<GetCotReportSeriesQuery> validator)
    : IRequestHandler<GetCotReportSeriesQuery, OperationalResult<GetCotReportSeriesResponse>>
{
    private static readonly IReadOnlyDictionary<string, ParticipantGroupDefinition> StandardGroups =
        new Dictionary<string, ParticipantGroupDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["dealer"] = new("dealer", "dealer_positions_long_all", "dealer_positions_short_all", "dealer_positions_spread_all"),
            ["asset-manager"] = new("asset-manager", "asset_mgr_positions_long", "asset_mgr_positions_short", "asset_mgr_positions_spread"),
            ["leveraged-money"] = new("leveraged-money", "lev_money_positions_long", "lev_money_positions_short", "lev_money_positions_spread"),
            ["other-reportables"] = new("other-reportables", "other_rept_positions_long", "other_rept_positions_short", "other_rept_positions_spread"),
            ["nonreportable"] = new("nonreportable", "nonrept_positions_long_all", "nonrept_positions_short_all", null)
        };

    private static readonly IReadOnlyDictionary<string, ParticipantGroupDefinition> LegacyGroups =
        new Dictionary<string, ParticipantGroupDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["non-commercial"] = new("non-commercial", "noncomm_positions_long_all", "noncomm_positions_short_all", "noncomm_postions_spread_all"),
            ["commercial"] = new("commercial", "comm_positions_long_all", "comm_positions_short_all", null),
            ["nonreportable"] = new("nonreportable", "nonrept_positions_long_all", "nonrept_positions_short_all", null)
        };

    public async Task<OperationalResult<GetCotReportSeriesResponse>> Handle(
        GetCotReportSeriesQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationalResult<GetCotReportSeriesResponse>.Fail(
                "Validation failed",
                "VALIDATION",
                validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .Distinct()
                    .ToArray());
        }

        try
        {
            var instrument = await instrumentRepository.GetBySlugAsync(request.InstrumentSlug, cancellationToken);
            if (instrument is null)
            {
                return OperationalResult<GetCotReportSeriesResponse>.Fail(
                    "Not found",
                    "NOT_FOUND",
                    $"Instrument '{request.InstrumentSlug}' was not found.");
            }

            var groupDefinition = GetGroupDefinition(request.Model, request.Group);
            if (groupDefinition is null)
            {
                return OperationalResult<GetCotReportSeriesResponse>.Fail(
                    "Validation failed",
                    "VALIDATION",
                    $"Group '{request.Group}' is not valid for model '{ToUiModelValue(request.Model)}'.");
            }

            var availableDates = await historicalReportRepository.GetAvailableDatesAsync(
                request.InstrumentSlug,
                request.Model,
                cancellationToken);
            if (availableDates.Count == 0)
            {
                return OperationalResult<GetCotReportSeriesResponse>.Fail(
                    "Collected report series not found",
                    "NOT_FOUND",
                    $"No collected {ToUiModelValue(request.Model)} series data was found for '{request.InstrumentSlug}'.");
            }

            var availableDateSet = availableDates.ToHashSet();
            if (!availableDateSet.Contains(request.FromDate) || !availableDateSet.Contains(request.ToDate))
            {
                return OperationalResult<GetCotReportSeriesResponse>.Fail(
                    "Validation failed",
                    "VALIDATION",
                    "Requested 'from' and 'to' dates must exist in the available report dates.");
            }

            var records = await historicalReportRepository.GetRangeAsync(
                request.InstrumentSlug,
                request.Model,
                request.FromDate,
                request.ToDate,
                cancellationToken);

            var items = records
                .Select(record => ToSeriesItem(record, groupDefinition))
                .Where(static item => item is not null)
                .Select(static item => item!)
                .OrderBy(x => x.ReportDate)
                .ToArray();

            var response = new GetCotReportSeriesResponse(
                instrument.Id,
                instrument.InstrumentSlug,
                instrument.InstrumentName,
                ToUiModelValue(request.Model),
                groupDefinition.Key,
                request.FromDate,
                request.ToDate,
                items);

            return OperationalResult<GetCotReportSeriesResponse>.Success(response, "Collected report series fetched successfully.");
        }
        catch (Exception ex)
        {
            return OperationalResult<GetCotReportSeriesResponse>.Fail(
                "Unexpected error",
                "UNEXPECTED",
                ex.Message);
        }
    }

    private static ParticipantGroupDefinition? GetGroupDefinition(CotReportModel model, string group)
    {
        var groups = model == CotReportModel.CotLegacy ? LegacyGroups : StandardGroups;
        return groups.TryGetValue(group, out var definition) ? definition : null;
    }

    private static GetCotReportSeriesItemResponse? ToSeriesItem(
        CotLatestReportRawRecord record,
        ParticipantGroupDefinition groupDefinition)
    {
        if (record.ReportDate is null)
        {
            return null;
        }

        int? longPosition = ParseInt(record.Fields, groupDefinition.LongPositionsKey);
        int? shortPosition = ParseInt(record.Fields, groupDefinition.ShortPositionsKey);
        int? spreadPosition = groupDefinition.SpreadPositionsKey is null
            ? null
            : ParseInt(record.Fields, groupDefinition.SpreadPositionsKey);

        return new GetCotReportSeriesItemResponse(
            DateOnly.FromDateTime(record.ReportDate.Value),
            longPosition,
            shortPosition,
            spreadPosition,
            longPosition - shortPosition,
            ParseInt(record.Fields, "open_interest_all"));
    }

    private static int? ParseInt(IReadOnlyDictionary<string, string?> fields, string key)
    {
        if (!fields.TryGetValue(key, out var rawValue) || string.IsNullOrWhiteSpace(rawValue))
        {
            return null;
        }

        return int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }

    private static string ToUiModelValue(CotReportModel model)
    {
        return model == CotReportModel.CotLegacy ? "cot-legacy" : "cot";
    }

    private sealed record ParticipantGroupDefinition(
        string Key,
        string LongPositionsKey,
        string ShortPositionsKey,
        string? SpreadPositionsKey);
}
