using Microsoft.Extensions.Localization;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.Admin.Dashboard;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.Admin.Dashboard;

public partial class AdminDashboard
{
    [Inject]
    private IStringLocalizer<AdminDashboardResource> L { get; set; } = default!;

    private IReadOnlyList<AdminSummaryCard> SummaryCards =>
    [
        new(L["Summary.Roles"], "3", L["Summary.RolesCaption"]),
        new(L["Summary.DefaultSignup"], "Guest", L["Summary.DefaultSignupCaption"]),
        new(L["Summary.AdminSeed"], "SuperAdmin", L["Summary.AdminSeedCaption"])
    ];

    private IReadOnlyList<AdminQueueItem> QueueItems =>
    [
        new(L["Queue.Item1.Title"], L["Queue.Item1.Description"]),
        new(L["Queue.Item2.Title"], L["Queue.Item2.Description"]),
        new(L["Queue.Item3.Title"], L["Queue.Item3.Description"])
    ];
}
