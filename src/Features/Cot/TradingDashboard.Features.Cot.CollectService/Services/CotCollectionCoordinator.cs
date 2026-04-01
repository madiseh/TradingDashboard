using Microsoft.AspNetCore.WebUtilities;
using TradingDashboard.Features.Cot.CollectService.Abstractions;
using TradingDashboard.Features.Cot.CollectService.Models;
using TradingDashboard.Features.Cot.CollectService.Options;

namespace TradingDashboard.Features.Cot.CollectService.Services;

public sealed class CotCollectionCoordinator : ICotCollectionCoordinator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CotCollectionCoordinator> _logger;
    private readonly CotCollectorOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private volatile bool _isRunning;
    private string _lastTriggerReason = "none";
    private DateTime? _currentRunStartedAtUtc;
    private DateTime? _nextScheduledRunAtUtc;

    public CotCollectionCoordinator(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        IOptions<CotCollectorOptions> options,
        ILogger<CotCollectionCoordinator> logger)
    {
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<CotCollectionStatusResponse> GetStatusAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CotDbContext>();
        var state = await dbContext.CotCollectionJobStates.FindAsync([CotInstrumentSeedBootstrapper.CollectorStateId], cancellationToken);

        return MapStatus(state);
    }

    public async Task<CotCollectionStatusResponse> RunOnceAsync(string reason, CancellationToken cancellationToken)
    {
        if (!_gate.Wait(0))
        {
            throw new InvalidOperationException("Collector is already running.");
        }

        _isRunning = true;
        _lastTriggerReason = string.IsNullOrWhiteSpace(reason) ? "manual" : reason.Trim();
        _currentRunStartedAtUtc = DateTime.UtcNow;

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CotDbContext>();
            var state = await EnsureJobStateAsync(dbContext, cancellationToken);
            state.RecordStart(_currentRunStartedAtUtc.Value);
            await dbContext.SaveChangesAsync(cancellationToken);

            var profiles = await dbContext.CotInstrumentCollectionProfiles
                .Include(x => x.Instrument)
                .OrderBy(x => x.Instrument!.InstrumentSlug)
                .ToListAsync(cancellationToken);

            var httpClient = _httpClientFactory.CreateClient(nameof(CotCollectionCoordinator));
            var storageRoot = ResolveStorageRoot();
            Directory.CreateDirectory(storageRoot);

            var savedFileCount = 0;
            var fetchedProfileCount = 0;
            var failures = new List<string>();

            foreach (var profile in profiles)
            {
                if (profile.Instrument is null)
                {
                    failures.Add($"Profile '{profile.Id}' has no linked instrument.");
                    continue;
                }

                if (_options.Normal.Enabled && profile.IsNormalEnabled)
                {
                    var result = await FetchAndStoreAsync(
                        httpClient,
                        profile,
                        profile.NormalMarketAndExchangeName,
                        _options.Normal,
                        CotReportModel.Cot,
                        storageRoot,
                        cancellationToken);

                    savedFileCount += result.SavedFiles;
                    fetchedProfileCount += result.WasSuccessful ? 1 : 0;
                    if (!result.WasSuccessful && result.ErrorMessage is not null)
                    {
                        failures.Add(result.ErrorMessage);
                    }
                }

                if (_options.Legacy.Enabled && profile.IsLegacyEnabled)
                {
                    var result = await FetchAndStoreAsync(
                        httpClient,
                        profile,
                        profile.LegacyMarketAndExchangeName,
                        _options.Legacy,
                        CotReportModel.CotLegacy,
                        storageRoot,
                        cancellationToken);

                    savedFileCount += result.SavedFiles;
                    fetchedProfileCount += result.WasSuccessful ? 1 : 0;
                    if (!result.WasSuccessful && result.ErrorMessage is not null)
                    {
                        failures.Add(result.ErrorMessage);
                    }
                }
            }

            var completedAtUtc = DateTime.UtcNow;
            if (failures.Count > 0)
            {
                state.RecordFailure(completedAtUtc, string.Join(" | ", failures.Take(5)), savedFileCount, fetchedProfileCount);
            }
            else
            {
                state.RecordSuccess(completedAtUtc, savedFileCount, fetchedProfileCount);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return MapStatus(state);
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

    private async Task<CotCollectionJobState> EnsureJobStateAsync(CotDbContext dbContext, CancellationToken cancellationToken)
    {
        var state = await dbContext.CotCollectionJobStates.FindAsync([CotInstrumentSeedBootstrapper.CollectorStateId], cancellationToken);
        if (state is not null)
        {
            return state;
        }

        state = new CotCollectionJobState(CotInstrumentSeedBootstrapper.CollectorStateId, DateTime.UtcNow);
        dbContext.CotCollectionJobStates.Add(state);
        return state;
    }

    private async Task<FetchResult> FetchAndStoreAsync(
        HttpClient httpClient,
        CotInstrumentCollectionProfile profile,
        string marketAndExchangeName,
        CotCollectorModeOptions modeOptions,
        CotReportModel model,
        string storageRoot,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(modeOptions.Dataset))
        {
            return FetchResult.Failure($"Dataset is not configured for model '{model}'.");
        }

        if (modeOptions.Limit <= 0)
        {
            return FetchResult.Failure($"Limit is not configured for model '{model}'.");
        }

        var instrumentSlug = profile.Instrument?.InstrumentSlug ?? profile.Id.ToString("N");
        var requestUri = BuildRequestUri(marketAndExchangeName, modeOptions);

        try
        {
            using var response = await httpClient.GetAsync(requestUri, cancellationToken);
            var payload = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return FetchResult.Failure(
                    $"Fetch failed for '{instrumentSlug}' ({model}) with status {(int)response.StatusCode}: {payload}");
            }

            JsonDocument.Parse(payload).Dispose();

            var modelFolder = model == CotReportModel.Cot ? "normal" : "legacy";
            var targetDirectory = Path.Combine(storageRoot, modelFolder, instrumentSlug);
            Directory.CreateDirectory(targetDirectory);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ");
            var versionedFilePath = Path.Combine(targetDirectory, $"{timestamp}.json");
            var latestFilePath = Path.Combine(targetDirectory, "latest.json");

            await File.WriteAllTextAsync(versionedFilePath, payload, cancellationToken);
            await File.WriteAllTextAsync(latestFilePath, payload, cancellationToken);

            _logger.LogInformation(
                "Saved raw COT payload for {InstrumentSlug} ({Model}) to {VersionedFilePath}",
                instrumentSlug,
                model,
                versionedFilePath);

            return FetchResult.Success();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return FetchResult.Failure($"Fetch failed for '{instrumentSlug}' ({model}): {ex.Message}");
        }
    }

    private string BuildRequestUri(string marketAndExchangeName, CotCollectorModeOptions modeOptions)
    {
        var query = new Dictionary<string, string?>
        {
            ["$where"] = $"market_and_exchange_names='{EscapeSingleQuotes(marketAndExchangeName)}'",
            ["$order"] = "report_date_as_yyyy_mm_dd DESC",
            ["$limit"] = modeOptions.Limit.ToString()
        };

        return QueryHelpers.AddQueryString($"/resource/{modeOptions.Dataset}.json", query);
    }

    private string ResolveStorageRoot()
    {
        return Path.IsPathRooted(_options.StorageRoot)
            ? _options.StorageRoot
            : Path.Combine(AppContext.BaseDirectory, _options.StorageRoot);
    }

    private CotCollectionStatusResponse MapStatus(CotCollectionJobState? state)
    {
        return new CotCollectionStatusResponse(
            _isRunning,
            _lastTriggerReason,
            _currentRunStartedAtUtc,
            _nextScheduledRunAtUtc,
            state?.LastStartedAtUtc,
            state?.LastCompletedAtUtc,
            state?.LastSucceededAtUtc,
            state?.LastSavedFileCount ?? 0,
            state?.LastFetchedProfileCount ?? 0,
            state?.LastError,
            _options.IntervalMinutes);
    }

    private static string EscapeSingleQuotes(string input)
    {
        return input.Replace("'", "''", StringComparison.Ordinal);
    }

    private readonly record struct FetchResult(bool WasSuccessful, int SavedFiles, string? ErrorMessage)
    {
        public static FetchResult Success() => new(true, 1, null);
        public static FetchResult Failure(string errorMessage) => new(false, 0, errorMessage);
    }
}
