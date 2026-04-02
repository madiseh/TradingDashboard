using System.Globalization;
using MudBlazor;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.Cot.Report;

public partial class CotReport
{
    private bool _isLoading = true;
    private string? _errorMessage;
    private CotLatestReportResponse? _report;
    private string? _lastInstrumentSlug;
    private string? _lastModel;
    private bool _hasExplicitDateSelection;

    private DateTime? _selectedAsOfDate;

    private DateTime? SelectedAsOfDate
    {
        get => _selectedAsOfDate;
        set => _selectedAsOfDate = NormalizeReportDate(value);
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
        if (!string.Equals(_lastInstrumentSlug, InstrumentSlug, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(_lastModel, Model, StringComparison.Ordinal))
        {
            _lastInstrumentSlug = InstrumentSlug;
            _lastModel = Model;
            _hasExplicitDateSelection = false;
            _selectedAsOfDate = null;
        }

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

    private string GetMarketAndExchangeLabel()
    {
        return string.IsNullOrWhiteSpace(_report?.MarketAndExchangeNames)
            ? GetDisplayName(InstrumentSlug)
            : _report.MarketAndExchangeNames;
    }

    private string GetContractUnitsLabel()
    {
        return string.IsNullOrWhiteSpace(_report?.ContractUnits)
            ? string.Empty
            : $"({_report.ContractUnits})";
    }

    private string GetInstrumentCodeLabel()
    {
        return string.IsNullOrWhiteSpace(_report?.InstrumentCode) ? "N/A" : _report.InstrumentCode;
    }

    private static string FormatNumber(int? value)
    {
        return value?.ToString("N0", CultureInfo.InvariantCulture) ?? "N/A";
    }

    private static string FormatSignedNumber(int? value)
    {
        if (value is null)
        {
            return "N/A";
        }

        return value.Value >= 0
            ? $"+{value.Value.ToString("N0", CultureInfo.InvariantCulture)}"
            : value.Value.ToString("N0", CultureInfo.InvariantCulture);
    }

    private static Color GetChangeColor(int? value)
    {
        return value switch
        {
            > 0 => Color.Success,
            < 0 => Color.Error,
            _ => Color.Default
        };
    }

    private static bool IsNotTuesday(DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Tuesday;
    }

    private static DateTime? NormalizeReportDate(DateTime? date)
    {
        if (date is null)
        {
            return null;
        }

        int offset = ((int)date.Value.DayOfWeek - (int)DayOfWeek.Tuesday + 7) % 7;
        return date.Value.Date.AddDays(-offset);
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

    private async Task OnSelectedAsOfDateChangedAsync(DateTime? value)
    {
        var normalizedDate = NormalizeReportDate(value);
        if (normalizedDate == SelectedAsOfDate && _hasExplicitDateSelection)
        {
            return;
        }

        _hasExplicitDateSelection = normalizedDate is not null;
        SelectedAsOfDate = normalizedDate;
        await LoadReportAsync();
    }

    private string BuildLatestReportUri()
    {
        var route = RoutePaths.Api.Cot.CotInstrumentLatestReport
            .Replace("{instrumentSlug}", Uri.EscapeDataString(InstrumentSlug), StringComparison.Ordinal);

        var queryParts = new List<string>
        {
            $"model={Uri.EscapeDataString(ReportModel.ToContractValue())}"
        };

        if (_hasExplicitDateSelection && SelectedAsOfDate is not null)
        {
            queryParts.Add($"reportDate={Uri.EscapeDataString(SelectedAsOfDate.Value.ToString("yyyy-MM-dd"))}");
        }

        return $"{route}?{string.Join("&", queryParts)}";
    }
}
