namespace TradingDashboard.Presentation.Blazor.Components.Pages.Cot.Home;

public partial class Home
{
    private static readonly string[] BothModels =
    [
        CotReportModel.CotLegacy.ToContractValue(),
        CotReportModel.Cot.ToContractValue()
    ];

    private bool _isLoading = true;
    private string? _errorMessage;
    private List<HomeGroupViewModel> _groups = [];

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
                _errorMessage = "Failed to load market directory.";
                _groups = [];
                return;
            }

            var payload = await response.Content.ReadFromJsonAsync<IReadOnlyList<GetCotInstrumentsByGroupResponse>>();
            if (payload is null)
            {
                _errorMessage = "Market directory response is empty.";
                _groups = [];
                return;
            }

            _groups = payload
                .Select(group => new HomeGroupViewModel(
                    group.Group ?? string.Empty,
                    (group.Items ?? [])
                        .Select(item => new HomeItemViewModel(
                            item.InstrumentName ?? string.Empty,
                            item.InstrumentSlug ?? string.Empty,
                            [.. BothModels]))
                        .ToList()))
                .ToList();
        }
        catch (Exception ex)
        {
            _errorMessage = $"Failed to load market directory. {ex.Message}";
            _groups = [];
        }
        finally
        {
            _isLoading = false;
        }
    }

    private static bool HasModel(HomeItemViewModel item, CotReportModel model)
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
