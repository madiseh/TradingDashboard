using System.Globalization;
using Microsoft.Extensions.Localization;
using TradingDashboard.Presentation.Blazor.Components.Pages.Workspace.Releases;
using TradingDashboard.Presentation.Common.Constants;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.Releases.Releases;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.Workspace.Releases;

public partial class Releases
{
    private static readonly DateOnly Version212ReleaseDate = new(2026, 4, 8);
    private static readonly DateOnly Version211ReleaseDate = new(2026, 4, 6);
    private static readonly DateOnly Version210ReleaseDate = new(2026, 4, 4);
    private static readonly DateOnly Version200ReleaseDate = new(2026, 4, 3);
    private static readonly DateOnly Version100ReleaseDate = new(2026, 4, 1);

    [Inject]
    private IStringLocalizer<ReleasesResource> L { get; set; } = default!;

    private string LatestReleaseDateLabel =>
        Version212ReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentUICulture);

    private IReadOnlyList<ReleaseEntryViewModel> ReleaseEntries =>
    [
        new(
            L["Entry.Version", ApplicationVersions.Blazor],
            Version212ReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentUICulture),
            L["Entry.Title.Version212"],
            L["Entry.Description.Version212"],
            true,
            [
                L["Entry.Version212.Highlight.1"],
                L["Entry.Version212.Highlight.2"],
                L["Entry.Version212.Highlight.3"],
                L["Entry.Version212.Highlight.4"],
                L["Entry.Version212.Highlight.5"]
            ]),
        new(
            L["Entry.Version", "2.1.1"],
            Version211ReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentUICulture),
            L["Entry.Title.Version211"],
            L["Entry.Description.Version211"],
            false,
            [
                L["Entry.Version211.Highlight.1"],
                L["Entry.Version211.Highlight.2"],
                L["Entry.Version211.Highlight.3"],
                L["Entry.Version211.Highlight.4"],
                L["Entry.Version211.Highlight.5"]
            ]),
        new(
            L["Entry.Version", "2.1.0"],
            Version210ReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentUICulture),
            L["Entry.Title.Version210"],
            L["Entry.Description.Version210"],
            false,
            [
                L["Entry.Version210.Highlight.1"],
                L["Entry.Version210.Highlight.2"],
                L["Entry.Version210.Highlight.3"]
            ]),
        new(
            L["Entry.Version", "2.0.0"],
            Version200ReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentUICulture),
            L["Entry.Title.Version2"],
            L["Entry.Description.Version2"],
            false,
            [
                L["Entry.Version2.Highlight.1"],
                L["Entry.Version2.Highlight.2"],
                L["Entry.Version2.Highlight.3"],
                L["Entry.Version2.Highlight.4"],
                L["Entry.Version2.Highlight.5"]
            ]),
        new(
            L["Entry.Version", "1.0.0"],
            Version100ReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentUICulture),
            L["Entry.Title.Version1"],
            L["Entry.Description.Version1"],
            false,
            [
                L["Entry.Version1.Highlight.1"],
                L["Entry.Version1.Highlight.2"],
                L["Entry.Version1.Highlight.3"],
                L["Entry.Version1.Highlight.4"],
                L["Entry.Version1.Highlight.5"]
            ])
    ];
}
