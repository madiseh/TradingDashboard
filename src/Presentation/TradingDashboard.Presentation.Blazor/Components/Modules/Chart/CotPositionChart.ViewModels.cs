using MudBlazor;

namespace TradingDashboard.Presentation.Blazor.Components.Modules.Chart;

public partial class CotPositionChart
{
    private enum PositionMetricType
    {
        NetPosition = 0,
        Long = 1,
        Short = 2
    }

    private sealed record MetricTabViewModel(
        PositionMetricType Type,
        string TabLabel,
        string Title,
        string Description,
        string EmptyMessage,
        string TooltipLabel,
        Color AccentColor);

    private sealed class GroupSelectionViewModel
    {
        public required string Key { get; init; }
        public required string Label { get; init; }
        public required string Color { get; init; }
        public bool IsSelected { get; set; }
    }

    private sealed record SeriesPointViewModel(
        DateOnly ReportDate,
        int Value,
        decimal X,
        decimal Y,
        string Tooltip);

    private sealed record ChartSeriesViewModel(
        string Key,
        string Label,
        string Color,
        string Path,
        int LatestValue,
        IReadOnlyList<SeriesPointViewModel> Points);

    private sealed record AxisLabelViewModel(
        decimal Position,
        string Text);
}
