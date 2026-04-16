namespace TradingDashboard.Presentation.Blazor.Components.Pages.TradingPanel._Dashboard;

public partial class TradingPanelDashboard
{
    private sealed record TradingMetricCard(string Label, string Value, string Caption);
    private sealed record TradingFocusItem(string Title, string Description);
}
