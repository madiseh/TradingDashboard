namespace TradingDashboard.Presentation.Blazor.Components.Pages.Cot.Report;

public partial class CotReport
{
    private DateTime? SelectedAsOfDate { get; set; } = DateTime.UtcNow.Date;

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
