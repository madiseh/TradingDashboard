namespace TradingDashboard.Presentation.Blazor.Components.Pages.Workspace.Markets;

public partial class Markets
{
    private sealed record MarketsGroupViewModel(
        string Name,
        IReadOnlyList<MarketsItemViewModel> Items);

    private sealed record MarketsItemViewModel(
        string Name,
        string Slug);
}
