namespace TradingDashboard.Presentation.Blazor.Components.Pages.Cot.Report;

public partial class CotReport
{
    private bool _isLoading = true;
    private string? _errorMessage;
    private CotLatestReportResponse? _report;

    private DateTime? _selectedAsOfDate = GetMostRecentTuesday(DateTime.UtcNow.Date);

    private DateTime? SelectedAsOfDate
    {
        get => _selectedAsOfDate;
        set => _selectedAsOfDate = value is null ? null : GetMostRecentTuesday(value.Value);
    }

    [Parameter]
    public string InstrumentSlug { get; set; } = string.Empty;

    [SupplyParameterFromQuery(Name = "model")]
    public string? Model { get; set; }

    private CotReportModel ReportModel =>
        CotReportModelExtensions.TryParse(Model, out CotReportModel parsed)
            ? parsed
            : CotReportModel.Cot;

    protected override async Task OnParametersSetAsync()
    {
        await LoadReportAsync();
    }

    private string GetBreadcrumbTitle()
    {
        return ReportModel == CotReportModel.CotLegacy
            ? "COT Legacy Report: Futures Only"
            : "Futures Only";
    }

    private string GetModelDescription()
    {
        return ReportModel == CotReportModel.CotLegacy
            ? "COT legacy report, futures only"
            : "futures only";
    }

    private string GetAsOfDateLabel()
    {
        return SelectedAsOfDate?.ToString("yyyy-MM-dd") ?? "N/A";
    }

    private static bool IsNotTuesday(DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Tuesday;
    }

    private static DateTime GetMostRecentTuesday(DateTime date)
    {
        int offset = ((int)date.DayOfWeek - (int)DayOfWeek.Tuesday + 7) % 7;
        return date.Date.AddDays(-offset);
    }
    private string GetDisplayName(string slug)
    {
        if (!string.IsNullOrWhiteSpace(_report?.InstrumentName))
        {
            return _report.InstrumentName;
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            return "UNKNOWN INSTRUMENT";
        }

        return string.Join(' ', slug.Split('-', StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();
    }

    private async Task LoadReportAsync()
    {
        _isLoading = true;
        _errorMessage = null;
        _report = null;

        try
        {
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiCot);
            var response = await client.GetAsync(BuildLatestReportUri());
            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = "Failed to load collected COT report.";
                return;
            }

            _report = await response.Content.ReadFromJsonAsync<CotLatestReportResponse>();
            if (_report is null)
            {
                _errorMessage = "Collected COT report response is empty.";
                return;
            }

            SelectedAsOfDate = _report.ReportDate;
        }
        catch (Exception ex)
        {
            _errorMessage = $"Failed to load collected COT report. {ex.Message}";
        }
        finally
        {
            _isLoading = false;
        }
    }

    private string BuildLatestReportUri()
    {
        var route = RoutePaths.Api.Cot.CotInstrumentLatestReport
            .Replace("{instrumentSlug}", Uri.EscapeDataString(InstrumentSlug), StringComparison.Ordinal);

        return $"{route}?model={Uri.EscapeDataString(ReportModel.ToContractValue())}";
    }
}
