namespace TradingDashboard.Presentation.Blazor.Components.Pages.Cot.Report;

public partial class CotReport
{
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

    private static string GetDisplayName(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return "UNKNOWN INSTRUMENT";
        }

        return string.Join(' ', slug.Split('-', StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();
    }
}
