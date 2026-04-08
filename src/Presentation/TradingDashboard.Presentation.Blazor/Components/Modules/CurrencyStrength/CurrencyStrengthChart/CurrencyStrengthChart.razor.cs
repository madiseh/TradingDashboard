using System.Globalization;
namespace TradingDashboard.Presentation.Blazor.Components.Modules.CurrencyStrength;

public partial class CurrencyStrengthChart
{
    private const decimal AxisPaddingFactor = 0.08m;
    private const decimal MinimumAxisPadding = 0.75m;
    private const decimal SvgWidth = 1480m;
    private const decimal SvgHeight = 560m;
    private const decimal PlotLeft = 86m;
    private const decimal PlotTop = 24m;
    private const decimal PlotRight = 36m;
    private const decimal PlotBottom = 72m;
    private const decimal TrailingPlotPaddingFactor = 0.05m;
    private const decimal VerticalPlotPaddingFactor = 0.04m;

    private static readonly IReadOnlyDictionary<string, string> CurrencyColors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["EUR"] = "#ff1f1f",
        ["AUD"] = "#1d4ed8",
        ["JPY"] = "#0ea5e9",
        ["CHF"] = "#a16207",
        ["NZD"] = "#ff38d1",
        ["GBP"] = "#10b981",
        ["USD"] = "#f59e0b",
        ["CAD"] = "#7c3aed"
    };

    [Parameter]
    public CurrencyStrengthSnapshotResponse? Snapshot { get; set; }

    [Parameter]
    public string EmptyMessage { get; set; } = "No data available.";

    private IReadOnlyList<CurrencyStrengthChartSeriesViewModel> ChartSeries => BuildChartSeries();
    private IReadOnlyList<CurrencyStrengthAxisLabelViewModel> YAxisLabels => BuildYAxisLabels();
    private IReadOnlyList<CurrencyStrengthAxisLabelViewModel> XAxisLabels => BuildXAxisLabels();
    private decimal ZeroLineY => GetZeroLineY();

    private IReadOnlyList<CurrencyStrengthChartSeriesViewModel> BuildChartSeries()
    {
        if (Snapshot?.Series is null || Snapshot.Series.Count == 0)
        {
            return [];
        }

        var allTimestamps = Snapshot.Series
            .SelectMany(series => series.Points)
            .Select(point => point.Timestamp)
            .Distinct()
            .OrderBy(value => value)
            .ToArray();

        if (allTimestamps.Length == 0)
        {
            return [];
        }

        var validSeries = Snapshot.Series
            .Where(series => !string.IsNullOrWhiteSpace(series.CurrencyCode) && series.Points.Count > 0)
            .OrderBy(series => series.CurrencyCode)
            .ToArray();

        var allValues = validSeries.SelectMany(series => series.Points).Select(point => point.Value).ToArray();
        GetChartScale(allValues, out var axisMin, out var axisMax, out _);

        var plotWidth = SvgWidth - PlotLeft - PlotRight;
        var plotHeight = SvgHeight - PlotTop - PlotBottom;
        var innerPlotTop = PlotTop + (plotHeight * VerticalPlotPaddingFactor);
        var innerPlotHeight = plotHeight * (1m - (VerticalPlotPaddingFactor * 2m));
        var range = axisMax - axisMin;
        var lastIndex = Math.Max(1, allTimestamps.Length - 1);
        var effectiveLastIndex = allTimestamps.Length == 1
            ? 1m
            : lastIndex / (1m - TrailingPlotPaddingFactor);

        return validSeries.Select(series =>
        {
            var points = series.Points
                .OrderBy(point => point.Timestamp)
                .Select(point =>
                {
                    var timestampIndex = Array.IndexOf(allTimestamps, point.Timestamp);
                    var x = PlotLeft + (plotWidth * timestampIndex / effectiveLastIndex);
                    var y = innerPlotTop + (innerPlotHeight * (axisMax - point.Value) / range);
                    var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(point.Timestamp).UtcDateTime;

                    return new CurrencyStrengthChartPointViewModel(
                        timestamp,
                        point.Value,
                        decimal.Round(x, 2),
                        decimal.Round(y, 2),
                        $"{series.CurrencyCode} | {timestamp:yyyy-MM-dd HH:mm} | {point.Value.ToString("0.###", CultureInfo.InvariantCulture)}");
                })
                .ToArray();

            return new CurrencyStrengthChartSeriesViewModel(
                series.CurrencyCode,
                CurrencyColors.TryGetValue(series.CurrencyCode, out var color) ? color : "#334155",
                string.Join(' ', points.Select(point => $"{point.X.ToString("0.##", CultureInfo.InvariantCulture)},{point.Y.ToString("0.##", CultureInfo.InvariantCulture)}")),
                points);
        }).ToArray();
    }

    private IReadOnlyList<CurrencyStrengthAxisLabelViewModel> BuildYAxisLabels()
    {
        var values = Snapshot?.Series.SelectMany(series => series.Points).Select(point => point.Value).ToArray() ?? [];
        if (values.Length == 0)
        {
            return [];
        }

        GetChartScale(values, out var axisMin, out var axisMax, out var step);
        var plotHeight = SvgHeight - PlotTop - PlotBottom;
        var innerPlotTop = PlotTop + (plotHeight * VerticalPlotPaddingFactor);
        var innerPlotHeight = plotHeight * (1m - (VerticalPlotPaddingFactor * 2m));
        var range = axisMax - axisMin;
        var labels = new List<CurrencyStrengthAxisLabelViewModel>();

        for (var value = axisMax; value >= axisMin; value -= step)
        {
            var y = innerPlotTop + (innerPlotHeight * (axisMax - value) / range);
            labels.Add(new CurrencyStrengthAxisLabelViewModel(decimal.Round(y, 2), value.ToString("0.##", CultureInfo.InvariantCulture)));
        }

        return labels;
    }

    private IReadOnlyList<CurrencyStrengthAxisLabelViewModel> BuildXAxisLabels()
    {
        var timestamps = Snapshot?.Series.SelectMany(series => series.Points).Select(point => point.Timestamp).Distinct().OrderBy(value => value).ToArray() ?? [];
        if (timestamps.Length == 0)
        {
            return [];
        }

        var tickCount = Math.Min(6, timestamps.Length);
        var plotWidth = SvgWidth - PlotLeft - PlotRight;
        var lastIndex = Math.Max(1, timestamps.Length - 1);
        var effectiveLastIndex = timestamps.Length == 1
            ? 1m
            : lastIndex / (1m - TrailingPlotPaddingFactor);
        var labels = new List<CurrencyStrengthAxisLabelViewModel>();

        foreach (var i in Enumerable.Range(0, tickCount))
        {
            var index = (int)Math.Round((timestamps.Length - 1d) * i / Math.Max(1, tickCount - 1));
            var x = PlotLeft + (plotWidth * index / effectiveLastIndex);
            var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestamps[index]).UtcDateTime;
            labels.Add(new CurrencyStrengthAxisLabelViewModel(decimal.Round(x, 2), timestamp.ToString("MM-dd HH:mm", CultureInfo.InvariantCulture)));
        }

        return labels.DistinctBy(label => label.Text).ToArray();
    }

    private decimal GetZeroLineY()
    {
        var values = Snapshot?.Series.SelectMany(series => series.Points).Select(point => point.Value).ToArray() ?? [];
        if (values.Length == 0)
        {
            return PlotTop;
        }

        GetChartScale(values, out var axisMin, out var axisMax, out _);
        var plotHeight = SvgHeight - PlotTop - PlotBottom;
        var innerPlotTop = PlotTop + (plotHeight * VerticalPlotPaddingFactor);
        var innerPlotHeight = plotHeight * (1m - (VerticalPlotPaddingFactor * 2m));
        var range = axisMax - axisMin;
        return innerPlotTop + (innerPlotHeight * axisMax / range);
    }

    private static void GetChartScale(
        IReadOnlyList<decimal> values,
        out decimal axisMin,
        out decimal axisMax,
        out decimal step)
    {
        var minValue = values.DefaultIfEmpty(0m).Min();
        var maxValue = values.DefaultIfEmpty(0m).Max();
        var rawRange = maxValue - minValue;

        if (rawRange <= 0m)
        {
            rawRange = 1m;
        }

        var padding = Math.Max(MinimumAxisPadding, rawRange * AxisPaddingFactor);
        var paddedMin = Math.Min(0m, minValue - padding);
        var paddedMax = Math.Max(0m, maxValue + padding);

        step = GetNiceStep((paddedMax - paddedMin) / 8m);
        axisMin = Math.Floor(paddedMin / step) * step;
        axisMax = Math.Ceiling(paddedMax / step) * step;
    }

    private static decimal GetNiceStep(decimal rawStep)
    {
        if (rawStep <= 0m)
        {
            return 1m;
        }

        var raw = (double)rawStep;
        var magnitude = Math.Pow(10, Math.Floor(Math.Log10(raw)));
        var residual = raw / magnitude;

        if (residual <= 1d) return (decimal)magnitude;
        if (residual <= 2d) return (decimal)(2d * magnitude);
        if (residual <= 2.5d) return (decimal)(2.5d * magnitude);
        if (residual <= 5d) return (decimal)(5d * magnitude);
        return (decimal)(10d * magnitude);
    }

    private static MarkupString BuildSvgText(decimal x, decimal y, string text, bool isDate, string anchor)
    {
        var cssClass = isDate ? "cot-net-position-axis-label date" : "cot-net-position-axis-label";
        return (MarkupString)$"<text x=\"{x.ToString("0.##", CultureInfo.InvariantCulture)}\" y=\"{y.ToString("0.##", CultureInfo.InvariantCulture)}\" text-anchor=\"{anchor}\" class=\"{cssClass}\">{System.Net.WebUtility.HtmlEncode(text)}</text>";
    }
}
