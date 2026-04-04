namespace TradingDashboard.Presentation.Blazor.Components.Modules.CurrencyStrength;

public sealed record CurrencyStrengthChartPointViewModel(
    DateTime Timestamp,
    decimal Value,
    decimal X,
    decimal Y,
    string Tooltip);

public sealed record CurrencyStrengthChartSeriesViewModel(
    string CurrencyCode,
    string Color,
    string Path,
    IReadOnlyList<CurrencyStrengthChartPointViewModel> Points);

public sealed record CurrencyStrengthAxisLabelViewModel(decimal Position, string Text);
