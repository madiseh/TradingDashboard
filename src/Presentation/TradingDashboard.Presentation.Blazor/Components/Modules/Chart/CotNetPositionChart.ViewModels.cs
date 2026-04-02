namespace TradingDashboard.Presentation.Blazor.Components.Modules.Chart;

public partial class CotNetPositionChart
{
    private sealed class GroupSelectionViewModel
    {
        public required string Key { get; init; }
        public required string Label { get; init; }
        public required string Color { get; init; }
        public bool IsSelected { get; set; }
    }

    private sealed record SeriesPointViewModel(
        DateOnly ReportDate,
        int NetPosition,
        decimal X,
        decimal Y,
        string Tooltip);

    private sealed record ChartSeriesViewModel(
        string Key,
        string Label,
        string Color,
        string Path,
        int LatestNetPosition,
        IReadOnlyList<SeriesPointViewModel> Points);

    private sealed record AxisLabelViewModel(
        decimal Position,
        string Text);
}
