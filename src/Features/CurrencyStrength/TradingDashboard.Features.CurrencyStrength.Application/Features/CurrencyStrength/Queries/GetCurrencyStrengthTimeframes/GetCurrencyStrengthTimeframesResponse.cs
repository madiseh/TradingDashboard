namespace TradingDashboard.Application.Features.CurrencyStrength.Queries.GetCurrencyStrengthTimeframes;

public sealed record GetCurrencyStrengthTimeframesResponse(
    IReadOnlyList<CurrencyStrengthTimeframeItemResponse> Items);

public sealed record CurrencyStrengthTimeframeItemResponse(
    string Key,
    string Label,
    string Description,
    int SortOrder,
    bool IsDefault);
