namespace TradingDashboard.Presentation.Blazor.Components.Modules.Table;

public partial class CotReportTable
{
    [Parameter]
    public string? Model { get; set; }

    private bool IsLegacy => string.Equals(Model, "CotLegacy", StringComparison.OrdinalIgnoreCase);
}
