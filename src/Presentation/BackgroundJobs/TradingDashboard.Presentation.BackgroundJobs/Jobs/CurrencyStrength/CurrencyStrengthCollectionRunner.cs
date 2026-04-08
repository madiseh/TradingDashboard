using System.Text.Json;
using Microsoft.Extensions.Options;

namespace TradingDashboard.Presentation.BackgroundJobs.Jobs.CurrencyStrength;

public sealed class CurrencyStrengthCollectionRunner
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CurrencyStrengthCollectionRunner> _logger;
    private readonly CurrencyStrengthCollectorOptions _options;

    public CurrencyStrengthCollectionRunner(
        IHttpClientFactory httpClientFactory,
        IOptions<CurrencyStrengthCollectorOptions> options,
        ILogger<CurrencyStrengthCollectionRunner> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<CurrencyStrengthCollectionResult> RunOnceAsync(CancellationToken cancellationToken)
    {
        if (_options.Timeframes.Count == 0)
        {
            throw new InvalidOperationException("CurrencyStrengthCollector:Timeframes must contain at least one item.");
        }

        var httpClient = _httpClientFactory.CreateClient(CurrencyStrengthJobRegistration.HttpClientName);
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

                var now = DateTime.UtcNow;
                var timeframeDirectory = Path.Combine(storageRoot, timeframe.Key);
                var snapshotsDirectory = Path.Combine(timeframeDirectory, "snapshots", now.ToString("yyyy-MM-dd"));

                Directory.CreateDirectory(timeframeDirectory);
                Directory.CreateDirectory(snapshotsDirectory);

                await File.WriteAllTextAsync(Path.Combine(snapshotsDirectory, $"{now:HH-mm-ss}.json"), payload, cancellationToken);
                await File.WriteAllTextAsync(Path.Combine(timeframeDirectory, "latest.json"), payload, cancellationToken);

                savedFileCount += 2;
                successfulFetchCount++;

                _logger.LogInformation("Collected currency strength snapshot for {TimeframeKey}", timeframe.Key);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                errors.Add($"{timeframe.Key}: {ex.Message}");
                _logger.LogWarning(ex, "Currency strength collection failed for timeframe {TimeframeKey}", timeframe.Key);
            }
        }

        var error = errors.Count == 0 ? null : string.Join(" | ", errors);

        if (successfulFetchCount == 0 && error is not null)
        {
            throw new InvalidOperationException(error);
        }

        if (error is not null)
        {
            _logger.LogWarning("Currency strength collection completed with partial errors: {Error}", error);
        }

        return new CurrencyStrengthCollectionResult(savedFileCount, successfulFetchCount, error);
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
}
