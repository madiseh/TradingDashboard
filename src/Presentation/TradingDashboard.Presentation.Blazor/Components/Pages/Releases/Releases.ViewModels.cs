namespace TradingDashboard.Presentation.Blazor.Components.Pages.Releases;

public sealed record ReleaseEntryViewModel(
    string DisplayDate,
    string Title,
    string Description,
    bool IsCurrent,
    IReadOnlyList<string> Highlights);
