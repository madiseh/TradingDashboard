namespace TradingDashboard.Presentation.Blazor.Components.Modules.Table;

public partial class CotReportTable
{
    [Parameter]
    public CotReportModel Model { get; set; } = CotReportModel.Cot;

    private bool IsLegacy => Model == CotReportModel.CotLegacy;
}
