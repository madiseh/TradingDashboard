using Microsoft.Extensions.Localization;
using TradingDashboard.Presentation.Blazor.Services;
using TradingDashboard.Presentation.Common.Resources.Components.Layout.AdminLayout;

namespace TradingDashboard.Presentation.Blazor.Components.Layout;

public partial class AdminLayout
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private AuthSession AuthSession { get; set; } = default!;

    [Inject]
    private PortalNavigationService PortalNavigation { get; set; } = default!;

    [Inject]
    private IStringLocalizer<AdminLayoutResource> L { get; set; } = default!;

    private bool _canRender;

    private IReadOnlyList<PortalNavigationItem> NavigationItems =>
    [
        new(L["Nav.Dashboard"], RoutePaths.Ui.Admin, "bi bi-grid-1x2-fill", true),
        new(L["Nav.Users"], null, "bi bi-people-fill", false),
        new(L["Nav.Access"], null, "bi bi-shield-lock-fill", false),
        new(L["Nav.Analytics"], null, "bi bi-bar-chart-fill", false)
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

        if (!PortalNavigation.CanAccessAdmin(AuthSession.Role))
        {
            NavigationManager.NavigateTo(PortalNavigation.GetDefaultPortalRoute(AuthSession.Role));
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
