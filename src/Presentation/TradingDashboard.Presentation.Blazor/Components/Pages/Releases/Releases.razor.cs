using System.Globalization;
using Microsoft.Extensions.Localization;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.Releases.Releases;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.Releases;

public partial class Releases
{
    private static readonly DateOnly LatestReleaseDate = new(2026, 4, 3);

    [Inject]
    private IStringLocalizer<ReleasesResource> L { get; set; } = default!;

    private string LatestReleaseDateLabel =>
        LatestReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentUICulture);

    private IReadOnlyList<ReleaseEntryViewModel> ReleaseEntries =>
    [
        new(
            LatestReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentUICulture),
            L["Entry.Title.CurrentSnapshot"],
            L["Entry.Description.CurrentSnapshot"],
            true,
            [
                L["Entry.Highlight.1"],
                L["Entry.Highlight.2"],
                L["Entry.Highlight.3"],
                L["Entry.Highlight.4"],
                L["Entry.Highlight.5"]
            ])
    ];
}
