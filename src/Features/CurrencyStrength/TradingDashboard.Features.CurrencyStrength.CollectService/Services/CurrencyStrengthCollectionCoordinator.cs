using System.Text.Json;
using TradingDashboard.Features.CurrencyStrength.CollectService.Abstractions;
using TradingDashboard.Features.CurrencyStrength.CollectService.Models;
using TradingDashboard.Features.CurrencyStrength.CollectService.Options;

namespace TradingDashboard.Features.CurrencyStrength.CollectService.Services;

public sealed class CurrencyStrengthCollectionCoordinator : ICurrencyStrengthCollectionCoordinator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CurrencyStrengthCollectionCoordinator> _logger;
    private readonly CurrencyStrengthCollectorOptions _options;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private volatile bool _isRunning;
    private string _lastTriggerReason = "none";
    private DateTime? _currentRunStartedAtUtc;
    private DateTime? _nextScheduledRunAtUtc;
    private DateTime? _lastCompletedAtUtc;
    private DateTime? _lastSucceededAtUtc;
    private int _lastSavedFileCount;
    private int _lastSuccessfulFetchCount;
    private string? _lastError;

    public CurrencyStrengthCollectionCoordinator(
        IHttpClientFactory httpClientFactory,
        IOptions<CurrencyStrengthCollectorOptions> options,
        ILogger<CurrencyStrengthCollectionCoordinator> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = options.Value;
    }

    public Task<CurrencyStrengthCollectionStatusResponse> GetStatusAsync(CancellationToken cancellationToken)
        => Task.FromResult(MapStatus());

    public async Task<CurrencyStrengthCollectionStatusResponse> RunOnceAsync(string reason, CancellationToken cancellationToken)
    {
        if (!_gate.Wait(0))
        {
            throw new InvalidOperationException("Collector is already running.");
        }

        _isRunning = true;
        _lastTriggerReason = string.IsNullOrWhiteSpace(reason) ? "manual" : reason.Trim();
        _currentRunStartedAtUtc = DateTime.UtcNow;
        _lastError = null;

        try
        {
            if (_options.Timeframes.Count == 0)
            {
                throw new InvalidOperationException("CurrencyStrengthCollector:Timeframes must contain at least one item.");
            }

            var httpClient = _httpClientFactory.CreateClient(nameof(CurrencyStrengthCollectionCoordinator));
            var storageRoot = ResolveStorageRoot();
            Directory.CreateDirectory(storageRoot);

            var savedFileCount = 0;
            var successfulFetchCount = 0;
            var errors = new List<string>();

            foreach (var timeframe in _options.Timeframes)
            {
                try
                {
                    var requestUri = $"{_options.BaseUrl}?t={Uri.EscapeDataString(timeframe.QueryValue)}";
                    using var response = await httpClient.GetAsync(requestUri, cancellationToken);
                    var payload = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (!response.IsSuccessStatusCode || !IsValidPayload(payload))
                    {
                        errors.Add($"{timeframe.Key}: fetch failed");
                        continue;
                    }

                    var timeframeDirectory = Path.Combine(storageRoot, timeframe.Key);
                    var snapshotsDirectory = Path.Combine(timeframeDirectory, "snapshots", DateTime.UtcNow.ToString("yyyy-MM-dd"));

                    Directory.CreateDirectory(timeframeDirectory);
                    Directory.CreateDirectory(snapshotsDirectory);

                    await File.WriteAllTextAsync(Path.Combine(snapshotsDirectory, $"{DateTime.UtcNow:HH-mm-ss}.json"), payload, cancellationToken);
                    await File.WriteAllTextAsync(Path.Combine(timeframeDirectory, "latest.json"), payload, cancellationToken);

                    savedFileCount += 2;
                    successfulFetchCount++;

                    _logger.LogInformation("Collected currency strength snapshot for {TimeframeKey}", timeframe.Key);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    errors.Add($"{timeframe.Key}: {ex.Message}");
                }
            }

            _lastCompletedAtUtc = DateTime.UtcNow;
            _lastSavedFileCount = savedFileCount;
            _lastSuccessfulFetchCount = successfulFetchCount;
            _lastError = errors.Count == 0 ? null : string.Join(" | ", errors);

            if (errors.Count == 0)
            {
                _lastSucceededAtUtc = _lastCompletedAtUtc;
            }

            return MapStatus();
        }
        finally
        {
            _isRunning = false;
            _currentRunStartedAtUtc = null;
            _gate.Release();
        }
    }

    public void SetNextScheduledRun(DateTime? nextScheduledRunAtUtc)
    {
        _nextScheduledRunAtUtc = nextScheduledRunAtUtc;
    }

    private string ResolveStorageRoot()
    {
        return Path.IsPathRooted(_options.StorageRoot)
            ? _options.StorageRoot
            : Path.Combine(AppContext.BaseDirectory, _options.StorageRoot);
    }

    private static bool IsValidPayload(string payload)
    {
        try
        {
            using var document = JsonDocument.Parse(payload);
            return document.RootElement.ValueKind == JsonValueKind.Array;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private CurrencyStrengthCollectionStatusResponse MapStatus()
    {
        return new CurrencyStrengthCollectionStatusResponse(
            _isRunning,
            _lastTriggerReason,
            _currentRunStartedAtUtc,
            _nextScheduledRunAtUtc,
            _lastCompletedAtUtc,
            _lastSucceededAtUtc,
            _lastSavedFileCount,
            _lastSuccessfulFetchCount,
            _lastError,
            _options.IntervalMinutes);
    }
}
