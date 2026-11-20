namespace TradingDashboard.Features.CurrencyStrength.Domain.ValueObjects;

public sealed record CurrencyStrengthPoint(long Timestamp, decimal Value);

public sealed record CurrencyStrengthSeries(string CurrencyCode, IReadOnlyList<CurrencyStrengthPoint> Points);

public sealed record CurrencyStrengthSnapshot(
    string TimeframeKey,
    string TimeframeLabel,
    DateTime CollectedAtUtc,
    IReadOnlyList<CurrencyStrengthSeries> Series);
