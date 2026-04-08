namespace TradingDashboard.Presentation.Blazor.Components.Pages.Admin.Users;

public partial class Users
{
    private sealed record AdminNavigationItemViewModel(
        string Label,
        string IconClass,
        string Href,
        bool IsAvailable,
        bool IsActive);

    private sealed record AdminSummaryCardViewModel(
        string Label,
        string Value,
        string Description);
}
