namespace TradingDashboard.Presentation.Blazor.Components.Pages.Workspace.Cot._Dashboard;

public partial class CotDashboard
{
    private sealed record CotDashboardGroupViewModel(
        string Type,
        IReadOnlyList<CotDashboardItemViewModel> Items);

    private sealed record CotDashboardItemViewModel(
        string Name,
        string Slug,
        IReadOnlyList<string> AvailableModels);
}
