using System.Globalization;
using Microsoft.Extensions.Localization;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.Releases.Releases;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.Releases;

public partial class Releases
{
    private static readonly DateOnly LatestReleaseDate = new(2026, 4, 3);
    private static readonly DateOnly PreviousReleaseDate = new(2026, 4, 1);

    [Inject]
    private IStringLocalizer<ReleasesResource> L { get; set; } = default!;

    private string LatestReleaseDateLabel =>
        LatestReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentUICulture);

    private IReadOnlyList<ReleaseEntryViewModel> ReleaseEntries =>
    [
        new(
            L["Entry.Version.2"],
            LatestReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentUICulture),
            L["Entry.Title.Version2"],
            L["Entry.Description.Version2"],
            true,
            [
                L["Entry.Version2.Highlight.1"],
                L["Entry.Version2.Highlight.2"],
                L["Entry.Version2.Highlight.3"],
                L["Entry.Version2.Highlight.4"],
                L["Entry.Version2.Highlight.5"]
            ]),
        new(
            L["Entry.Version.1"],
            PreviousReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentUICulture),
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
