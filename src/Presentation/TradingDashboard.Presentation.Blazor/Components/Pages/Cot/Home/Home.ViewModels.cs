namespace TradingDashboard.Presentation.Blazor.Components.Pages.Cot.Home;

public partial class Home
{
    private sealed record HomeGroupViewModel(
        string Type,
        IReadOnlyList<HomeItemViewModel> Items);

    private sealed record HomeItemViewModel(
        string Name,
        string Slug,
        IReadOnlyList<string> AvailableModels);
}
