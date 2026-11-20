namespace TradingDashboard.Application.Features.CurrencyStrength.Queries.GetCurrencyStrengthSnapshot;

public sealed record GetCurrencyStrengthSnapshotResponse(
    string TimeframeKey,
    string TimeframeLabel,
    string Description,
    DateTime CollectedAtUtc,
    DateTime? LastPointAtUtc,
    IReadOnlyList<CurrencyStrengthSeriesResponse> Series);

public sealed record CurrencyStrengthSeriesResponse(
    string CurrencyCode,
    IReadOnlyList<CurrencyStrengthPointResponse> Points);

public sealed record CurrencyStrengthPointResponse(long Timestamp, decimal Value);
