using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.Markets.Markets;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.Workspace.Markets;

public partial class Markets
{
    private bool _isLoading = true;
    private string? _errorMessage;
    private MarketsGroupViewModel? _group;
    private string? _lastGroup;
    private string? _lastModel;

    [Inject]
    private IStringLocalizer<MarketsResource> L { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "group")]
    public string? Group { get; set; }

    [SupplyParameterFromQuery(Name = "model")]
    public string? Model { get; set; }

    private CotReportModel SelectedModel =>
        CotReportModelExtensions.TryParse(Model, out CotReportModel parsed)
            ? parsed
            : CotReportModel.Cot;

    protected override async Task OnParametersSetAsync()
    {
        if (string.Equals(_lastGroup, Group, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(_lastModel, Model, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _lastGroup = Group;
        _lastModel = Model;

        await LoadGroupAsync();
    }

    private async Task LoadGroupAsync()
    {
        _isLoading = true;
        _errorMessage = null;
        _group = null;

        if (string.IsNullOrWhiteSpace(Group))
        {
            _errorMessage = L["Errors.GroupMissing"];
            _isLoading = false;
            return;
        }

        try
        {
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiCot);
            var response = await client.GetAsync(BuildGroupUri(Group));
            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = L["Errors.LoadGroup"];
                return;
            }

            var payload = await response.Content.ReadFromJsonAsync<IReadOnlyList<GetCotInstrumentsByGroupResponse>>();
            if (payload is null)
            {
                _errorMessage = L["Errors.EmptyGroupResponse"];
                return;
            }

            var matchingGroup = payload.FirstOrDefault(group =>
                string.Equals(group.Group, Group, StringComparison.OrdinalIgnoreCase));

            if (matchingGroup is null)
            {
                _errorMessage = L["Errors.GroupNotFound", Group];
                return;
            }

            _group = new MarketsGroupViewModel(
                matchingGroup.Group ?? Group,
                (matchingGroup.Items ?? [])
                    .Where(item => !string.IsNullOrWhiteSpace(item.InstrumentSlug))
                    .Select(item => new MarketsItemViewModel(
                        item.InstrumentName ?? item.InstrumentSlug ?? string.Empty,
                        item.InstrumentSlug ?? string.Empty))
                    .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList());
        }
        catch (Exception ex)
        {
            _errorMessage = L["Errors.LoadGroupWithMessage", ex.Message];
        }
        finally
        {
            _isLoading = false;
        }
    }

    private string GetPageTitle()
    {
        return string.IsNullOrWhiteSpace(Group)
            ? L["PageTitle"]
            : L["PageTitle.WithGroup", Group];
    }

    private string GetHeading()
    {
        return _group?.Name ?? Group ?? L["Heading.Markets"];
    }

    private string GetSelectedModelLabel()
    {
        return SelectedModel == CotReportModel.CotLegacy
            ? L["Models.CotLegacy"]
            : L["Models.Cot"];
    }

    private string BuildCotLink(string slug)
    {
        return $"{RoutePaths.Ui.CotReports}/{slug}?model={SelectedModel.ToContractValue()}";
    }

    private static string BuildGroupUri(string group)
    {
        return $"{RoutePaths.Api.Cot.CotInstrumentsByGroup}?group={Uri.EscapeDataString(group)}";
    }
}
