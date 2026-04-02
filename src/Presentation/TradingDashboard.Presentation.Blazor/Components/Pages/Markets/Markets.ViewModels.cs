namespace TradingDashboard.Presentation.Blazor.Components.Pages.Markets;

public partial class Markets
{
    private sealed record MarketsGroupViewModel(
        string Name,
        IReadOnlyList<MarketsItemViewModel> Items);

    private sealed record MarketsItemViewModel(
        string Name,
        string Slug);
}
