using Microsoft.Extensions.Localization;
using TradingDashboard.Presentation.Blazor.Services;
using TradingDashboard.Presentation.Common.Resources.Components.Layout.TradingPanelLayout;

namespace TradingDashboard.Presentation.Blazor.Components.Layout;

public partial class TradingPanelLayout
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private AuthSession AuthSession { get; set; } = default!;

    [Inject]
    private PortalNavigationService PortalNavigation { get; set; } = default!;

    [Inject]
    private IStringLocalizer<TradingPanelLayoutResource> L { get; set; } = default!;

    private bool _canRender;

    private IReadOnlyList<PortalNavigationItem> NavigationItems =>
    [
        new(L["Nav.Dashboard"], RoutePaths.Ui.TradingPanel, "bi bi-speedometer2", true),
        new(L["Nav.Signals"], null, "bi bi-activity", false),
        new(L["Nav.Macro"], null, "bi bi-globe2", false),
        new(L["Nav.Playbook"], null, "bi bi-journal-richtext", false)
    ];

    protected override void OnParametersSet()
    {
        _canRender = EnsureAuthorized();
    }

    private bool EnsureAuthorized()
    {
        if (!AuthSession.IsAuthenticated)
        {
            var currentPath = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).PathAndQuery;
            NavigationManager.NavigateTo(PortalNavigation.BuildLoginUrl(currentPath));
            return false;
        }

        if (!PortalNavigation.CanAccessTradingPanel(AuthSession.Role))
        {
            NavigationManager.NavigateTo(RoutePaths.Ui.Home);
            return false;
        }

        return true;
    }

    private void Logout()
    {
        AuthSession.Clear();
        NavigationManager.NavigateTo(RoutePaths.Ui.Home);
    }

    private sealed record PortalNavigationItem(string Label, string? Href, string Icon, bool IsEnabled);
}
