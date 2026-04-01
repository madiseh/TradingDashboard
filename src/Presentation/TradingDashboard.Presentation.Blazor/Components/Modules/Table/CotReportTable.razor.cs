namespace TradingDashboard.Presentation.Blazor.Components.Modules.Table;

public partial class CotReportTable
{
    [Parameter]
    public CotReportModel Model { get; set; } = CotReportModel.Cot;

    [Parameter]
    public CotLatestReportResponse? Report { get; set; }

    private bool IsLegacy => Model == CotReportModel.CotLegacy;

    private IReadOnlyList<CotLatestReportCategoryResponse> Categories => Report?.Categories ?? [];

    private CotLatestReportCategoryResponse? GetCategory(string key)
    {
        return Categories.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
    }

    private static string FormatNumber(int? value)
    {
        return value?.ToString("N0") ?? string.Empty;
    }

    private static string FormatPercent(decimal? value)
    {
        return value.HasValue ? $"{value.Value:0.0}%" : string.Empty;
    }

    private static string FormatSigned(int? value)
    {
        return value.HasValue ? value.Value.ToString("+#,##0;-#,##0;0") : string.Empty;
    }

    private static string GetChangeCss(int? value)
    {
        return value switch
        {
            > 0 => "pill positive",
            < 0 => "pill negative",
            _ => "pill"
        };
    }
}
