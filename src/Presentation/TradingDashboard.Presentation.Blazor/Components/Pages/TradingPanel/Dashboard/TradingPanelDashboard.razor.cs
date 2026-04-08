using Microsoft.Extensions.Localization;
using TradingDashboard.Presentation.Blazor.Services;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.TradingPanel.Dashboard;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.TradingPanel.Dashboard;

public partial class TradingPanelDashboard
{
    [Inject]
    private IStringLocalizer<TradingPanelDashboardResource> L { get; set; } = default!;

    [Inject]
    private AuthSession AuthSession { get; set; } = default!;

    private IReadOnlyList<TradingMetricCard> Metrics =>
    [
        new(L["Metrics.Role"], AuthSession.Role ?? "-", L["Metrics.RoleCaption"]),
        new(L["Metrics.Readiness"], L["Metrics.ReadinessValue"], L["Metrics.ReadinessCaption"]),
        new(L["Metrics.Access"], L["Metrics.AccessValue"], L["Metrics.AccessCaption"])
    ];

    private IReadOnlyList<TradingFocusItem> FocusItems =>
    [
        new(L["Focus.Item1.Title"], L["Focus.Item1.Description"]),
        new(L["Focus.Item2.Title"], L["Focus.Item2.Description"]),
        new(L["Focus.Item3.Title"], L["Focus.Item3.Description"])
    ];
}
