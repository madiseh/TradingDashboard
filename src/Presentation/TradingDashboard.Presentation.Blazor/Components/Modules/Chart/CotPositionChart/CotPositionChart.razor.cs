using System.Globalization;
using MudBlazor;

namespace TradingDashboard.Presentation.Blazor.Components.Modules.Chart;

public partial class CotPositionChart
{
    private const int DefaultVisibleWeeks = 52;
    private const decimal SvgWidth = 960m;
    private const decimal SvgHeight = 420m;
    private const decimal PlotLeft = 72m;
    private const decimal PlotTop = 20m;
    private const decimal PlotRight = 28m;
    private const decimal PlotBottom = 56m;

    private static readonly IReadOnlyList<GroupSelectionViewModel> StandardGroups =
    [
        new() { Key = "dealer", Label = "Dealer", Color = "#006d77", IsSelected = true },
        new() { Key = "asset-manager", Label = "Asset Manager", Color = "#1d4ed8", IsSelected = true },
        new() { Key = "leveraged-money", Label = "Leveraged Funds", Color = "#b45309", IsSelected = true },
        new() { Key = "other-reportables", Label = "Other Reportables", Color = "#7c3aed", IsSelected = true },
        new() { Key = "nonreportable", Label = "Non-Reportable", Color = "#be123c", IsSelected = true }
    ];

    private static readonly IReadOnlyList<GroupSelectionViewModel> DisaggregatedGroups =
    [
        new() { Key = "producer-merchant", Label = "Producer/Merchant", Color = "#006d77", IsSelected = true },
        new() { Key = "swap-dealers", Label = "Swap Dealers", Color = "#1d4ed8", IsSelected = true },
        new() { Key = "managed-money", Label = "Managed Money", Color = "#b45309", IsSelected = true },
        new() { Key = "other-reportables", Label = "Other Reportables", Color = "#7c3aed", IsSelected = true },
        new() { Key = "nonreportable", Label = "Non-Reportable", Color = "#be123c", IsSelected = true }
    ];

    private static readonly IReadOnlyList<GroupSelectionViewModel> LegacyGroups =
    [
        new() { Key = "non-commercial", Label = "Non-Commercial", Color = "#006d77", IsSelected = true },
        new() { Key = "commercial", Label = "Commercial", Color = "#1d4ed8", IsSelected = true },
        new() { Key = "nonreportable", Label = "Non-Reportable", Color = "#be123c", IsSelected = true }
    ];

    private static readonly IReadOnlyList<MetricTabViewModel> MetricTabs =
    [
        new(
            PositionMetricType.NetPosition,
            "Net Position",
            "Net Position",
            "Select participant groups and a report-date range to compare net positioning.",
            "Select at least one participant group to render the net position chart.",
            "Net",
            Color.Primary),
        new(
            PositionMetricType.Long,
            "Long",
            "Long Positions",
            "Use the same participant and date filters to compare long positioning across groups.",
            "Select at least one participant group to render the long positions chart.",
            "Long",
            Color.Info),
        new(
            PositionMetricType.Short,
            "Short",
            "Short Positions",
            "Use the same participant and date filters to compare short positioning across groups.",
            "Select at least one participant group to render the short positions chart.",
            "Short",
            Color.Warning)
    ];

    private bool _isLoading = true;
    private string? _errorMessage;
    private string? _lastContextKey;
    private int _activeMetricTabIndex;
    private List<GroupSelectionViewModel> _groupSelections = [];
    private List<DateOnly> _availableDates = [];
    private Dictionary<string, CotReportSeriesResponse> _seriesByGroup = new(StringComparer.OrdinalIgnoreCase);
    private string? _instrumentType;
    private DateTime? _fromDate;
    private DateTime? _toDate;

    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = default!;

    [Parameter]
    public string InstrumentSlug { get; set; } = string.Empty;

    [Parameter]
    public CotReportModel Model { get; set; } = CotReportModel.Cot;

    private IReadOnlyList<GroupSelectionViewModel> SelectedGroups => _groupSelections.Where(x => x.IsSelected).ToArray();

    private MetricTabViewModel SelectedMetric => MetricTabs[_activeMetricTabIndex];

    private DateTime? MinAvailableDate => _availableDates.Count == 0
        ? null
        : _availableDates[0].ToDateTime(TimeOnly.MinValue);

    private DateTime? MaxAvailableDate => _availableDates.Count == 0
        ? null
        : _availableDates[^1].ToDateTime(TimeOnly.MinValue);

    private string SelectedRangeLabel => _fromDate is null || _toDate is null
        ? "N/A"
        : $"{_fromDate:yyyy-MM-dd} to {_toDate:yyyy-MM-dd}";

    private IReadOnlyList<ChartSeriesViewModel> ChartSeries => BuildChartSeries();

    private IReadOnlyList<AxisLabelViewModel> YAxisLabels => BuildYAxisLabels();

    private IReadOnlyList<AxisLabelViewModel> XAxisLabels => BuildXAxisLabels();

    private decimal ZeroLineY => GetZeroLineY();

    protected override async Task OnParametersSetAsync()
    {
        var contextKey = $"{InstrumentSlug}|{Model}";
        if (string.Equals(_lastContextKey, contextKey, StringComparison.Ordinal))
        {
            return;
        }

        _lastContextKey = contextKey;
        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        _isLoading = true;
        _errorMessage = null;
        _activeMetricTabIndex = 0;
        _seriesByGroup.Clear();
        _availableDates.Clear();
        _instrumentType = null;

        try
        {
            await LoadInstrumentAsync();
            _groupSelections = CreateDefaultGroupSelections();
            await LoadAvailableDatesAsync();
            SetDefaultRange();
            await LoadSeriesAsync();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private List<GroupSelectionViewModel> CreateDefaultGroupSelections()
    {
        var source = Model == CotReportModel.CotLegacy
            ? LegacyGroups
            : IsDisaggregatedInstrumentType(_instrumentType)
                ? DisaggregatedGroups
                : StandardGroups;

        return source
            .Select(x => new GroupSelectionViewModel
            {
                Key = x.Key,
                Label = x.Label,
                Color = x.Color,
                IsSelected = true
            })
            .ToList();
    }

    private async Task LoadInstrumentAsync()
    {
        if (Model == CotReportModel.CotLegacy)
        {
            return;
        }

        var client = HttpClientFactory.CreateClient(HttpClientNames.ApiCot);
        var response = await client.GetAsync(BuildInstrumentUri());
        if (!response.IsSuccessStatusCode)
        {
            return;
        }

        var payload = await response.Content.ReadFromJsonAsync<GetCotInstrumentBySlugResponse>();
        _instrumentType = payload?.Type;
    }

    private static bool IsDisaggregatedInstrumentType(string? instrumentType)
    {
        return string.Equals(instrumentType, "Energies", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(instrumentType, "Grains", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(instrumentType, "Livestock & Dairy", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(instrumentType, "Metals", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(instrumentType, "Softs", StringComparison.OrdinalIgnoreCase);
    }

    private async Task LoadAvailableDatesAsync()
    {
        var client = HttpClientFactory.CreateClient(HttpClientNames.ApiCot);
        var response = await client.GetAsync(BuildAvailableDatesUri());
        if (!response.IsSuccessStatusCode)
        {
            _errorMessage = "Failed to load available report dates for the position chart.";
            return;
        }

        var payload = await response.Content.ReadFromJsonAsync<CotAvailableDatesResponse>();
        if (payload is null || payload.AvailableDates.Count == 0)
        {
            _errorMessage = "No available report dates were returned for the position chart.";
            return;
        }

        _availableDates = payload.AvailableDates
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    private void SetDefaultRange()
    {
        if (_availableDates.Count == 0)
        {
            _fromDate = null;
            _toDate = null;
            return;
        }

        var toDate = _availableDates[^1];
        var fromIndex = Math.Max(0, _availableDates.Count - DefaultVisibleWeeks);
        var fromDate = _availableDates[fromIndex];

        _fromDate = fromDate.ToDateTime(TimeOnly.MinValue);
        _toDate = toDate.ToDateTime(TimeOnly.MinValue);
    }

    private async Task LoadSeriesAsync()
    {
        if (_errorMessage is not null || _fromDate is null || _toDate is null)
        {
            return;
        }

        var selectedGroups = SelectedGroups;
        if (selectedGroups.Count == 0)
        {
            _seriesByGroup.Clear();
            return;
        }

        var client = HttpClientFactory.CreateClient(HttpClientNames.ApiCot);
        var results = await Task.WhenAll(selectedGroups.Select(group => LoadSeriesForGroupAsync(client, group)));

        var failedResult = results.FirstOrDefault(x => x.Response is null);
        if (failedResult.Group is not null)
        {
            _errorMessage = $"Failed to load chart series for '{failedResult.Group.Label}'.";
            return;
        }

        _seriesByGroup = results
            .Where(x => x.Response is not null)
            .ToDictionary(
                x => x.Group.Key,
                x => x.Response!,
                StringComparer.OrdinalIgnoreCase);
    }

    private async Task<(GroupSelectionViewModel Group, CotReportSeriesResponse? Response)> LoadSeriesForGroupAsync(
        HttpClient client,
        GroupSelectionViewModel group)
    {
        var response = await client.GetAsync(BuildSeriesUri(group.Key));
        if (!response.IsSuccessStatusCode)
        {
            return (group, null);
        }

        return (group, await response.Content.ReadFromJsonAsync<CotReportSeriesResponse>());
    }

    private void OnActiveMetricTabChanged(int index)
    {
        if (index < 0 || index >= MetricTabs.Count)
        {
            return;
        }

        _activeMetricTabIndex = index;
    }

    private async Task OnGroupSelectionChangedAsync(string groupKey, bool selected)
    {
        var group = _groupSelections.FirstOrDefault(x => string.Equals(x.Key, groupKey, StringComparison.OrdinalIgnoreCase));
        if (group is null || group.IsSelected == selected)
        {
            return;
        }

        group.IsSelected = selected;
        _errorMessage = null;
        await LoadSeriesAsync();
    }

    private async Task ToggleGroupSelectionAsync(string groupKey)
    {
        var group = _groupSelections.FirstOrDefault(x => string.Equals(x.Key, groupKey, StringComparison.OrdinalIgnoreCase));
        if (group is null)
        {
            return;
        }

        await OnGroupSelectionChangedAsync(groupKey, !group.IsSelected);
    }

    private async Task OnFromDateChangedAsync(DateTime? value)
    {
        var normalized = NormalizeAvailableDate(value);
        if (normalized is null || normalized == _fromDate)
        {
            return;
        }

        _fromDate = normalized;
        if (_toDate is not null && _fromDate > _toDate)
        {
            _toDate = _fromDate;
        }

        _errorMessage = null;
        await LoadSeriesAsync();
    }

    private async Task OnToDateChangedAsync(DateTime? value)
    {
        var normalized = NormalizeAvailableDate(value);
        if (normalized is null || normalized == _toDate)
        {
            return;
        }

        _toDate = normalized;
        if (_fromDate is not null && _toDate < _fromDate)
        {
            _fromDate = _toDate;
        }

        _errorMessage = null;
        await LoadSeriesAsync();
    }

    private DateTime? NormalizeAvailableDate(DateTime? value)
    {
        if (value is null || _availableDates.Count == 0)
        {
            return null;
        }

        var date = DateOnly.FromDateTime(value.Value);
        return _availableDates.Contains(date)
            ? date.ToDateTime(TimeOnly.MinValue)
            : null;
    }

    private bool IsDateUnavailable(DateTime value)
    {
        return !_availableDates.Contains(DateOnly.FromDateTime(value));
    }

    private IReadOnlyList<ChartSeriesViewModel> BuildChartSeries()
    {
        if (_seriesByGroup.Count == 0)
        {
            return [];
        }

        var allDates = _seriesByGroup.Values
            .SelectMany(x => x.Items)
            .Select(x => x.ReportDate)
            .Distinct()
            .OrderBy(x => x)
            .ToArray();

        if (allDates.Length == 0)
        {
            return [];
        }

        var selectedSeries = _groupSelections
            .Where(x => x.IsSelected)
            .Where(x => _seriesByGroup.ContainsKey(x.Key))
            .Select(group => new
            {
                Group = group,
                Points = _seriesByGroup[group.Key].Items
                    .Select(item => new
                    {
                        Item = item,
                        Value = GetSelectedMetricValue(item)
                    })
                    .Where(x => x.Value is not null)
                    .Select(x => new
                    {
                        x.Item.ReportDate,
                        Value = x.Value!.Value
                    })
                    .OrderBy(item => item.ReportDate)
                    .ToArray()
            })
            .Where(x => x.Points.Length > 0)
            .ToArray();

        if (selectedSeries.Length == 0)
        {
            return [];
        }

        var allValues = selectedSeries
            .SelectMany(x => x.Points)
            .Select(x => x.Value)
            .ToArray();

        GetChartScale(allValues, out var axisMin, out var axisMax, out _);
        var plotWidth = SvgWidth - PlotLeft - PlotRight;
        var plotHeight = SvgHeight - PlotTop - PlotBottom;
        var range = axisMax - axisMin;
        var lastIndex = Math.Max(1, allDates.Length - 1);

        return selectedSeries
            .Select(series =>
            {
                var points = series.Points
                    .Select(item =>
                    {
                        var dateIndex = Array.IndexOf(allDates, item.ReportDate);
                        var x = PlotLeft + (plotWidth * dateIndex / lastIndex);
                        var y = PlotTop + (plotHeight * (decimal)(axisMax - item.Value) / (decimal)range);
                        var tooltip = $"{series.Group.Label} | {item.ReportDate:yyyy-MM-dd} | {SelectedMetric.TooltipLabel}: {FormatNumber(item.Value)}";

                        return new SeriesPointViewModel(
                            item.ReportDate,
                            item.Value,
                            decimal.Round(x, 2),
                            decimal.Round(y, 2),
                            tooltip);
                    })
                    .ToArray();

                return new ChartSeriesViewModel(
                    series.Group.Key,
                    series.Group.Label,
                    series.Group.Color,
                    string.Join(' ', points.Select(point => $"{point.X.ToString("0.##", CultureInfo.InvariantCulture)},{point.Y.ToString("0.##", CultureInfo.InvariantCulture)}")),
                    points[^1].Value,
                    points);
            })
            .ToArray();
    }

    private IReadOnlyList<AxisLabelViewModel> BuildYAxisLabels()
    {
        var values = ChartSeries.SelectMany(x => x.Points).Select(x => x.Value).ToArray();
        if (values.Length == 0)
        {
            return [];
        }

        GetChartScale(values, out var axisMin, out var axisMax, out var step);
        var plotHeight = SvgHeight - PlotTop - PlotBottom;
        var range = axisMax - axisMin;
        var labels = new List<AxisLabelViewModel>();

        for (var value = axisMax; value >= axisMin; value -= step)
        {
            var y = PlotTop + (plotHeight * (decimal)(axisMax - value) / (decimal)range);
            labels.Add(new AxisLabelViewModel(decimal.Round(y, 2), FormatCompactNumber((int)Math.Round(value))));
        }

        if (labels.Count == 0)
        {
            labels.Add(new AxisLabelViewModel(PlotTop + plotHeight / 2m, "0"));
        }

        return labels;
    }

    private IReadOnlyList<AxisLabelViewModel> BuildXAxisLabels()
    {
        var allDates = ChartSeries
            .SelectMany(x => x.Points)
            .Select(x => x.ReportDate)
            .Distinct()
            .OrderBy(x => x)
            .ToArray();

        if (allDates.Length == 0)
        {
            return [];
        }

        var tickCount = Math.Min(5, allDates.Length);
        var plotWidth = SvgWidth - PlotLeft - PlotRight;
        var lastIndex = Math.Max(1, allDates.Length - 1);
        var labels = new List<AxisLabelViewModel>(tickCount);

        if (tickCount == 1)
        {
            labels.Add(new AxisLabelViewModel(PlotLeft + plotWidth / 2m, allDates[0].ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            return labels;
        }

        foreach (var i in Enumerable.Range(0, tickCount))
        {
            var index = (int)Math.Round((allDates.Length - 1d) * i / (tickCount - 1));
            var x = PlotLeft + (plotWidth * index / lastIndex);
            labels.Add(new AxisLabelViewModel(
                decimal.Round(x, 2),
                allDates[index].ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        return labels
            .DistinctBy(x => x.Text)
            .ToArray();
    }

    private decimal GetZeroLineY()
    {
        var values = ChartSeries.SelectMany(x => x.Points).Select(x => x.Value).ToArray();
        if (values.Length == 0)
        {
            return PlotTop;
        }

        GetChartScale(values, out var axisMin, out var axisMax, out _);
        var plotHeight = SvgHeight - PlotTop - PlotBottom;
        var range = axisMax - axisMin;
        return PlotTop + (plotHeight * (decimal)axisMax / (decimal)range);
    }

    private static void GetChartScale(
        IReadOnlyList<int> values,
        out double axisMin,
        out double axisMax,
        out double step)
    {
        var minValue = Math.Min(0, values.Min());
        var maxValue = Math.Max(0, values.Max());
        if (minValue == maxValue)
        {
            minValue -= 1;
            maxValue += 1;
        }

        step = GetNiceStep((maxValue - minValue) / 4d);
        axisMin = Math.Floor(minValue / step) * step;
        axisMax = Math.Ceiling(maxValue / step) * step;
        if (axisMin == axisMax)
        {
            axisMax += step;
        }
    }

    private static double GetNiceStep(double rawStep)
    {
        if (rawStep <= 0)
        {
            return 1;
        }

        var magnitude = Math.Pow(10, Math.Floor(Math.Log10(rawStep)));
        var residual = rawStep / magnitude;

        if (residual <= 1)
        {
            return magnitude;
        }

        if (residual <= 2)
        {
            return 2 * magnitude;
        }

        if (residual <= 5)
        {
            return 5 * magnitude;
        }

        return 10 * magnitude;
    }

    private string BuildAvailableDatesUri()
    {
        var route = RoutePaths.Api.Cot.CotInstrumentAvailableDates
            .Replace("{instrumentSlug}", Uri.EscapeDataString(InstrumentSlug), StringComparison.Ordinal);

        return $"{route}?model={Uri.EscapeDataString(Model.ToContractValue())}";
    }

    private string BuildInstrumentUri()
    {
        return RoutePaths.Api.Cot.CotInstrumentBySlug
            .Replace("{instrumentSlug}", Uri.EscapeDataString(InstrumentSlug), StringComparison.Ordinal);
    }

    private string BuildSeriesUri(string groupKey)
    {
        var route = RoutePaths.Api.Cot.CotInstrumentSeries
            .Replace("{instrumentSlug}", Uri.EscapeDataString(InstrumentSlug), StringComparison.Ordinal);

        return $"{route}?model={Uri.EscapeDataString(Model.ToContractValue())}&group={Uri.EscapeDataString(groupKey)}&from={Uri.EscapeDataString(_fromDate!.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}&to={Uri.EscapeDataString(_toDate!.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}";
    }

    private int? GetSelectedMetricValue(CotReportSeriesItemResponse item)
    {
        return SelectedMetric.Type switch
        {
            PositionMetricType.Long => item.Long,
            PositionMetricType.Short => item.Short,
            _ => item.NetPosition
        };
    }

    private static string FormatNumber(int? value)
    {
        return value?.ToString("N0", CultureInfo.InvariantCulture) ?? "N/A";
    }

    private static string FormatCompactNumber(int value)
    {
        var absolute = Math.Abs(value);
        if (absolute >= 1_000_000)
        {
            return $"{value / 1_000_000d:0.#}M";
        }

        if (absolute >= 1_000)
        {
            return $"{value / 1_000d:0.#}K";
        }

        return value.ToString("N0", CultureInfo.InvariantCulture);
    }

    private string GetChartAriaLabel()
    {
        return $"{SelectedMetric.Title} chart";
    }

    private Color GetSelectedMetricAccentColor()
    {
        return SelectedMetric.AccentColor;
    }

    private string GetGroupToggleClass(GroupSelectionViewModel group)
    {
        return group.IsSelected
            ? "cot-net-position-legend-item is-selected"
            : "cot-net-position-legend-item is-muted";
    }

    private string GetGroupLatestValueLabel(GroupSelectionViewModel group)
    {
        return group.IsSelected
            ? FormatNumber(GetGroupLatestMetricValue(group))
            : "Off";
    }

    private string GetGroupLatestValueClass(GroupSelectionViewModel group)
    {
        if (!group.IsSelected)
        {
            return "cot-net-position-value is-off";
        }

        var latestValue = GetGroupLatestMetricValue(group);
        if (latestValue is null)
        {
            return "cot-net-position-value is-neutral";
        }

        if (SelectedMetric.Type != PositionMetricType.NetPosition)
        {
            return "cot-net-position-value is-accent";
        }

        return latestValue.Value switch
        {
            > 0 => "cot-net-position-value is-positive",
            < 0 => "cot-net-position-value is-negative",
            _ => "cot-net-position-value is-neutral"
        };
    }

    private int? GetGroupLatestMetricValue(GroupSelectionViewModel group)
    {
        if (!_seriesByGroup.TryGetValue(group.Key, out var response))
        {
            return null;
        }

        return response.Items
            .OrderBy(x => x.ReportDate)
            .Select(GetSelectedMetricValue)
            .LastOrDefault(x => x is not null);
    }

    private static MarkupString BuildSvgText(decimal x, decimal y, string text, bool isDate, string anchor)
    {
        var cssClass = isDate ? "cot-net-position-axis-label date" : "cot-net-position-axis-label";
        return (MarkupString)$"<text x=\"{x.ToString("0.##", CultureInfo.InvariantCulture)}\" y=\"{y.ToString("0.##", CultureInfo.InvariantCulture)}\" text-anchor=\"{anchor}\" class=\"{cssClass}\">{System.Net.WebUtility.HtmlEncode(text)}</text>";
    }
}
