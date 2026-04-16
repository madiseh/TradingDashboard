namespace TradingDashboard.Presentation.Blazor.Components.Pages.Workspace.Releases;

public sealed record ReleaseEntryViewModel(
    string VersionLabel,
    string DisplayDate,
    string Title,
    string Description,
    bool IsCurrent,
    IReadOnlyList<string> Highlights);
