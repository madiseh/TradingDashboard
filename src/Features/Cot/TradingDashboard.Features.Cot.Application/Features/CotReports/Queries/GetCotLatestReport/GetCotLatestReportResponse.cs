namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotLatestReport;

public sealed record GetCotLatestReportResponse(
    Guid InstrumentId,
    string InstrumentCode,
    string InstrumentSlug,
    string InstrumentName,
    string Model,
    string MarketAndExchangeNames,
    DateTime? ReportDate,
    string ContractUnits,
    int? OpenInterestAll,
    int? ChangeInOpenInterestAll,
    int? TotalTraders,
    IReadOnlyList<GetCotLatestReportCategoryResponse> Categories);

public sealed record GetCotLatestReportCategoryResponse(
    string Key,
    string Label,
    GetCotLatestReportMetricResponse? Long,
    GetCotLatestReportMetricResponse? Short,
    GetCotLatestReportMetricResponse? Spread);

public sealed record GetCotLatestReportMetricResponse(
    int? Positions,
    decimal? PercentOfOpenInterest,
    int? Traders,
    int? Change);
