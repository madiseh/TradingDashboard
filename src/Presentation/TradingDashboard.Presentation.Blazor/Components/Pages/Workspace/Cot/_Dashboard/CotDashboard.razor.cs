using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.Cot.Dashboard;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.Workspace.Cot._Dashboard;

public partial class CotDashboard
{
    private static readonly string[] BothModels =
    [
        CotReportModel.CotLegacy.ToContractValue(),
        CotReportModel.Cot.ToContractValue()
    ];

    private bool _isLoading = true;
    private string? _errorMessage;
    private List<CotDashboardGroupViewModel> _groups = [];

    [Inject]
    private IStringLocalizer<CotDashboardResource> L { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadDirectoryAsync();
    }

    private async Task LoadDirectoryAsync()
    {
        _isLoading = true;
        _errorMessage = null;

        try
        {
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiCot);
            var response = await client.GetAsync(RoutePaths.Api.Cot.CotInstrumentsByGroup);
            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = L["Errors.LoadDirectory"];
                _groups = [];
                return;
            }

            var payload = await response.Content.ReadFromJsonAsync<IReadOnlyList<GetCotInstrumentsByGroupResponse>>();
            if (payload is null)
            {
                _errorMessage = L["Errors.EmptyDirectoryResponse"];
                _groups = [];
                return;
            }

            _groups = payload
                .Select(group => new CotDashboardGroupViewModel(
                    group.Group ?? string.Empty,
                    (group.Items ?? [])
                        .Select(item => new CotDashboardItemViewModel(
                            item.InstrumentName ?? string.Empty,
                            item.InstrumentSlug ?? string.Empty,
                            [.. BothModels]))
                        .ToList()))
                .ToList();
        }
        catch (Exception ex)
        {
            _errorMessage = L["Errors.LoadDirectoryWithMessage", ex.Message];
            _groups = [];
        }
        finally
        {
            _isLoading = false;
        }
    }

    private static bool HasModel(CotDashboardItemViewModel item, CotReportModel model)
    {
        return item.AvailableModels.Contains(model.ToContractValue(), StringComparer.OrdinalIgnoreCase);
    }

    private static string BuildCotLink(string slug, CotReportModel model)
    {
        return $"{RoutePaths.Ui.CotReports}/{slug}?model={model.ToContractValue()}";
    }

    private static string BuildMarketsLink(string group, CotReportModel model)
    {
        return $"{RoutePaths.Ui.Markets}?group={Uri.EscapeDataString(group)}&model={model.ToContractValue()}";
    }
}
