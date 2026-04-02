namespace TradingDashboard.Presentation.Blazor.Components.Modules.Chart;

public partial class CotLongShortChart
{
    private sealed record ChartBarViewModel(
        string Key,
        string Label,
        int LongPositions,
        int ShortPositions,
        decimal LongRatio,
        decimal ShortRatio);
}
