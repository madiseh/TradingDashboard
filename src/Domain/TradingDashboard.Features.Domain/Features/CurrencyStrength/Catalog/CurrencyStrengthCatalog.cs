namespace TradingDashboard.Features.CurrencyStrength.Domain.Catalog;

public sealed record CurrencyStrengthTimeframeDefinition(
    string Key,
    string Label,
    string Description,
    int SortOrder,
    bool IsDefault,
    string QueryValue);

public static class CurrencyStrengthCatalog
{
    public static readonly IReadOnlyList<CurrencyStrengthTimeframeDefinition> All =
    [
        new("today", "Today", "Today", 1, false, "today"),
        new("lastday", "Last Day", "Last day", 2, false, "lastday"),
        new("10min", "10m", "Last 10 minutes", 3, false, "10min"),
        new("30min", "30m", "Last 30 minutes", 4, true, "30min"),
        new("1hour", "1H", "Last 1 hour", 5, false, "1hour"),
        new("4hour", "4H", "Last 4 hours", 6, false, "4hour"),
        new("8hour", "8H", "Last 8 hours", 7, false, "8hour"),
        new("12hour", "12H", "Last 12 hours", 8, false, "12hour"),
        new("24hour", "24H", "Last 24 hours", 9, false, "24hour"),
        new("48hour", "48H", "Last 48 hours", 10, false, "48hour"),
        new("72hour", "72H", "Last 72 hours", 11, false, "72hour"),
        new("1week", "1W", "Last 1 week", 12, false, "1week"),
        new("1mon", "1Mon", "Last 1 month", 13, false, "1mon"),
        new("lastmon", "Last Mon", "Previous month", 14, false, "lastmon"),
        new("3mon", "3Mon", "Last 3 months", 15, false, "3mon"),
        new("1y", "1Y", "Last 1 year", 16, false, "1y"),
        new("3y", "3Y", "Last 3 years", 17, false, "3y"),
        new("5y", "5Y", "Last 5 years", 18, false, "5y"),
        new("10y", "10Y", "Last 10 years", 19, false, "10y")
    ];

    public static bool TryGet(string key, out CurrencyStrengthTimeframeDefinition timeframe)
    {
        timeframe = All.FirstOrDefault(item => string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
            ?? All.First(item => item.IsDefault);

        return All.Any(item => string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase));
    }
}
