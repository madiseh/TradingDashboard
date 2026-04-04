using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.CurrencyStrength.Dashboard;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.CurrencyStrength.Dashboard;

public partial class CurrencyStrengthDashboard
    : IAsyncDisposable
{
    private const string ChartAnchorId = "currency-strength-chart-anchor";
    private readonly CancellationTokenSource _autoRefreshCts = new();
    private bool _isLoadingTimeframes = true;
    private bool _isLoadingSnapshot;
    private bool _scrollChartIntoViewRequested;
    private string? _errorMessage;
    private string? _selectedTimeframeKey;
    private CurrencyStrengthSnapshotResponse? _snapshot;
    private Task? _autoRefreshTask;
    private List<CurrencyStrengthTimeframeViewModel> _timeframes = [];

    [Inject]
    private IStringLocalizer<CurrencyStrengthDashboardResource> L { get; set; } = default!;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await ReloadAsync();
        _autoRefreshTask = StartAutoRefreshLoopAsync(_autoRefreshCts.Token);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_scrollChartIntoViewRequested)
        {
            return;
        }

        _scrollChartIntoViewRequested = false;
        await JS.InvokeVoidAsync("tradingDashboardViewport.scrollElementIntoViewById", ChartAnchorId);
    }

    private async Task ReloadAsync()
    {
        _isLoadingTimeframes = true;
        _errorMessage = null;

        try
        {
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiCurrencyStrength);
            var payload = await client.GetFromJsonAsync<CurrencyStrengthTimeframesResponse>(RoutePaths.Api.CurrencyStrength.Timeframes);
            if (payload?.Items is null || payload.Items.Count == 0)
            {
                _errorMessage = L["Errors.LoadTimeframes"];
                _timeframes = [];
                return;
            }

            _timeframes = payload.Items
                .OrderBy(item => item.SortOrder)
                .Select(item => new CurrencyStrengthTimeframeViewModel(item.Key, item.Label, item.Description, item.IsDefault))
                .ToList();

            _selectedTimeframeKey = _timeframes.FirstOrDefault(item => item.IsDefault)?.Key ?? _timeframes[0].Key;
            await LoadSnapshotAsync(_selectedTimeframeKey);
        }
        catch (Exception ex)
        {
            _errorMessage = L["Errors.LoadTimeframesWithMessage", ex.Message];
            _timeframes = [];
        }
        finally
        {
            _isLoadingTimeframes = false;
        }
    }

    private async Task OnTimeframeSelectedAsync(string timeframeKey)
    {
        if (string.Equals(_selectedTimeframeKey, timeframeKey, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _selectedTimeframeKey = timeframeKey;
        _scrollChartIntoViewRequested = true;
        await LoadSnapshotAsync(timeframeKey);
    }

    private async Task LoadSnapshotAsync(string timeframeKey, bool silentRefresh = false)
    {
        if (!silentRefresh)
        {
            _isLoadingSnapshot = true;
            _errorMessage = null;
        }

        try
        {
            var route = RoutePaths.Api.CurrencyStrength.Latest.Replace("{timeframeKey}", Uri.EscapeDataString(timeframeKey), StringComparison.Ordinal);
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiCurrencyStrength);
            _snapshot = await client.GetFromJsonAsync<CurrencyStrengthSnapshotResponse>(route);

            if (_snapshot is null)
            {
                _errorMessage = L["Errors.LoadSnapshot"];
            }
        }
        catch (Exception ex)
        {
            if (!silentRefresh)
            {
                _snapshot = null;
                _errorMessage = L["Errors.LoadSnapshotWithMessage", ex.Message];
            }
        }
        finally
        {
            if (!silentRefresh)
            {
                _isLoadingSnapshot = false;
            }
        }
    }

    private async Task StartAutoRefreshLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                if (string.IsNullOrWhiteSpace(_selectedTimeframeKey) || _isLoadingTimeframes || _isLoadingSnapshot)
                {
                    continue;
                }

                await InvokeAsync(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(_selectedTimeframeKey))
                    {
                        await LoadSnapshotAsync(_selectedTimeframeKey, silentRefresh: true);
                        StateHasChanged();
                    }
                });
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_autoRefreshCts.IsCancellationRequested)
        {
            await _autoRefreshCts.CancelAsync();
        }

        if (_autoRefreshTask is not null)
        {
            try
            {
                await _autoRefreshTask;
            }
            catch (OperationCanceledException)
            {
            }
        }

        _autoRefreshCts.Dispose();
    }
}
