namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotReportSeries;

public sealed record GetCotReportSeriesResponse(
    Guid InstrumentId,
    string InstrumentSlug,
    string InstrumentName,
    string Model,
    string Group,
    DateOnly FromDate,
    DateOnly ToDate,
    IReadOnlyList<GetCotReportSeriesItemResponse> Items);

public sealed record GetCotReportSeriesItemResponse(
    DateOnly ReportDate,
    int? Long,
    int? Short,
    int? Spread,
    int? NetPosition,
    int? OpenInterest);
