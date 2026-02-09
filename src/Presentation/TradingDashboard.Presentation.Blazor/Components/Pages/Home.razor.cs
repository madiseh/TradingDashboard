namespace TradingDashboard.Presentation.Blazor.Components.Pages;

public partial class Home
{
    private bool _isLoading = true;
    private string? _errorMessage;
    private IReadOnlyList<TradingDashboard.Presentation.Blazor.Features.Common.Contracts.MarketGroupDto> _groups = [];

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
            var response = await CommonDirectoryClient.GetMarketsDirectoryAsync();
            _groups = response?.Groups ?? [];
        }
        catch
        {
            _errorMessage = "Failed to load market directory from API.";
            _groups = [];
        }
        finally
        {
            _isLoading = false;
        }
    }

    private static bool HasModel(TradingDashboard.Presentation.Blazor.Features.Common.Contracts.MarketItemDto item, CotReportModel model)
    {
        var expectedValue = model.ToContractValue();
        return item.AvailableModels.Any(x => string.Equals(x, expectedValue, StringComparison.OrdinalIgnoreCase));
    }

    private static string BuildCotLink(string slug, CotReportModel model)
    {
        return $"{RoutePaths.Ui.Cot}/{slug}?model={model.ToContractValue()}";
    }

    private static string BuildMarketsLink(string type, CotReportModel model)
    {
        return $"{RoutePaths.Ui.Markets}/{type}?model={model.ToContractValue()}";
    }
}
