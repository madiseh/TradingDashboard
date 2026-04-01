using System.Globalization;
using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Models;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Repository.Cot;

namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotLatestReport;

public sealed class GetCotLatestReportHandler(
    ICotInstrumentQueryRepository instrumentRepository,
    ICotLatestReportQueryRepository latestReportRepository,
    IValidator<GetCotLatestReportQuery> validator)
    : IRequestHandler<GetCotLatestReportQuery, OperationalResult<GetCotLatestReportResponse>>
{
    public async Task<OperationalResult<GetCotLatestReportResponse>> Handle(
        GetCotLatestReportQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationalResult<GetCotLatestReportResponse>.Fail(
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
                return OperationalResult<GetCotLatestReportResponse>.Fail(
                    "Not found",
                    "NOT_FOUND",
                    $"Instrument '{request.InstrumentSlug}' was not found.");
            }

            var latestReport = await latestReportRepository.GetAsync(
                request.InstrumentSlug,
                request.Model,
                request.ReportDate,
                cancellationToken);
            if (latestReport is null)
            {
                var reportDateMessage = request.ReportDate is null
                    ? string.Empty
                    : $" for {request.ReportDate.Value:yyyy-MM-dd}";

                return OperationalResult<GetCotLatestReportResponse>.Fail(
                    "Collected report not found",
                    "NOT_FOUND",
                    $"No collected {ToUiModelValue(request.Model)} report{reportDateMessage} was found for '{request.InstrumentSlug}'.");
            }

            var response = new GetCotLatestReportResponse(
                instrument.Id,
                instrument.InstrumentId,
                instrument.InstrumentSlug,
                instrument.InstrumentName,
                ToUiModelValue(request.Model),
                latestReport.MarketAndExchangeNames ?? string.Empty,
                latestReport.ReportDate,
                latestReport.ContractUnits ?? string.Empty,
                ParseInt(latestReport.Fields, "open_interest_all"),
                ParseInt(latestReport.Fields, "change_in_open_interest_all"),
                ParseInt(latestReport.Fields, "traders_tot_all"),
                BuildCategories(request.Model, latestReport.Fields));

            return OperationalResult<GetCotLatestReportResponse>.Success(response, "Collected report fetched successfully.");
        }
        catch (Exception ex)
        {
            return OperationalResult<GetCotLatestReportResponse>.Fail(
                "Unexpected error",
                "UNEXPECTED",
                ex.Message);
        }
    }

    private static IReadOnlyList<GetCotLatestReportCategoryResponse> BuildCategories(
        CotReportModel model,
        IReadOnlyDictionary<string, string?> fields)
    {
        return model switch
        {
            CotReportModel.CotLegacy => BuildLegacyCategories(fields),
            _ => BuildStandardCategories(fields)
        };
    }

    private static IReadOnlyList<GetCotLatestReportCategoryResponse> BuildStandardCategories(
        IReadOnlyDictionary<string, string?> fields)
    {
        return
        [
            new(
                "dealer",
                "Dealer Intermediary",
                BuildMetric(fields, "dealer_positions_long_all", "pct_of_oi_dealer_long_all", "traders_dealer_long_all", "change_in_dealer_long_all"),
                BuildMetric(fields, "dealer_positions_short_all", "pct_of_oi_dealer_short_all", "traders_dealer_short_all", "change_in_dealer_short_all"),
                BuildMetric(fields, "dealer_positions_spread_all", "pct_of_oi_dealer_spread_all", "traders_dealer_spread_all", "change_in_dealer_spread_all")),
            new(
                "asset-manager",
                "Asset Manager/Institutional",
                BuildMetric(fields, "asset_mgr_positions_long", "pct_of_oi_asset_mgr_long", "traders_asset_mgr_long_all", "change_in_asset_mgr_long"),
                BuildMetric(fields, "asset_mgr_positions_short", "pct_of_oi_asset_mgr_short", "traders_asset_mgr_short_all", "change_in_asset_mgr_short"),
                BuildMetric(fields, "asset_mgr_positions_spread", "pct_of_oi_asset_mgr_spread", "traders_asset_mgr_spread", "change_in_asset_mgr_spread")),
            new(
                "leveraged-money",
                "Leveraged Funds",
                BuildMetric(fields, "lev_money_positions_long", "pct_of_oi_lev_money_long", "traders_lev_money_long_all", "change_in_lev_money_long"),
                BuildMetric(fields, "lev_money_positions_short", "pct_of_oi_lev_money_short", "traders_lev_money_short_all", "change_in_lev_money_short"),
                BuildMetric(fields, "lev_money_positions_spread", "pct_of_oi_lev_money_spread", "traders_lev_money_spread", "change_in_lev_money_spread")),
            new(
                "other-reportables",
                "Other Reportables",
                BuildMetric(fields, "other_rept_positions_long", "pct_of_oi_other_rept_long", "traders_other_rept_long_all", "change_in_other_rept_long"),
                BuildMetric(fields, "other_rept_positions_short", "pct_of_oi_other_rept_short", "traders_other_rept_short", "change_in_other_rept_short"),
                BuildMetric(fields, "other_rept_positions_spread", "pct_of_oi_other_rept_spread", "traders_other_rept_spread", "change_in_other_rept_spread")),
            new(
                "nonreportable",
                "Nonreportable Positions",
                BuildMetric(fields, "nonrept_positions_long_all", "pct_of_oi_nonrept_long_all", null, "change_in_nonrept_long_all"),
                BuildMetric(fields, "nonrept_positions_short_all", "pct_of_oi_nonrept_short_all", null, "change_in_nonrept_short_all"),
                null)
        ];
    }

    private static IReadOnlyList<GetCotLatestReportCategoryResponse> BuildLegacyCategories(
        IReadOnlyDictionary<string, string?> fields)
    {
        return
        [
            new(
                "non-commercial",
                "Non-Commercial",
                BuildMetric(fields, "noncomm_positions_long_all", "pct_of_oi_noncomm_long_all", "traders_noncomm_long_all", "change_in_noncomm_long_all"),
                BuildMetric(fields, "noncomm_positions_short_all", "pct_of_oi_noncomm_short_all", "traders_noncomm_short_all", "change_in_noncomm_short_all"),
                BuildMetric(fields, "noncomm_postions_spread_all", "pct_of_oi_noncomm_spread", "traders_noncomm_spread_all", "change_in_noncomm_spead_all")),
            new(
                "commercial",
                "Commercial",
                BuildMetric(fields, "comm_positions_long_all", "pct_of_oi_comm_long_all", "traders_comm_long_all", "change_in_comm_long_all"),
                BuildMetric(fields, "comm_positions_short_all", "pct_of_oi_comm_short_all", "traders_comm_short_all", "change_in_comm_short_all"),
                null),
            new(
                "total",
                "Total",
                BuildMetric(fields, "tot_rept_positions_long_all", "pct_of_oi_tot_rept_long_all", "traders_tot_rept_long_all", "change_in_tot_rept_long_all"),
                BuildMetric(fields, "tot_rept_positions_short", "pct_of_oi_tot_rept_short", "traders_tot_rept_short_all", "change_in_tot_rept_short"),
                null),
            new(
                "nonreportable",
                "Non-Reportable",
                BuildMetric(fields, "nonrept_positions_long_all", "pct_of_oi_nonrept_long_all", null, "change_in_nonrept_long_all"),
                BuildMetric(fields, "nonrept_positions_short_all", "pct_of_oi_nonrept_short_all", null, "change_in_nonrept_short_all"),
                null)
        ];
    }

    private static GetCotLatestReportMetricResponse BuildMetric(
        IReadOnlyDictionary<string, string?> fields,
        string positionsKey,
        string percentKey,
        string? tradersKey,
        string changeKey)
    {
        return new GetCotLatestReportMetricResponse(
            ParseInt(fields, positionsKey),
            ParseDecimal(fields, percentKey),
            tradersKey is null ? null : ParseInt(fields, tradersKey),
            ParseInt(fields, changeKey));
    }

    private static int? ParseInt(IReadOnlyDictionary<string, string?> fields, string key)
    {
        if (!fields.TryGetValue(key, out var rawValue) || string.IsNullOrWhiteSpace(rawValue))
        {
            return null;
        }

        return int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;
    }

    private static decimal? ParseDecimal(IReadOnlyDictionary<string, string?> fields, string key)
    {
        if (!fields.TryGetValue(key, out var rawValue) || string.IsNullOrWhiteSpace(rawValue))
        {
            return null;
        }

        return decimal.TryParse(rawValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;
    }

    private static string ToUiModelValue(CotReportModel model)
    {
        return model == CotReportModel.CotLegacy ? "cot-legacy" : "cot";
    }
}
