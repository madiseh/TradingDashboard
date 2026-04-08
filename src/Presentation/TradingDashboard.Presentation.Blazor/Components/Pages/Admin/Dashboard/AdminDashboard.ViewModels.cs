namespace TradingDashboard.Presentation.Blazor.Components.Pages.Admin.Dashboard;

public partial class AdminDashboard
{
    private sealed record AdminSummaryCard(string Label, string Value, string Caption);
    private sealed record AdminQueueItem(string Title, string Description);
}
