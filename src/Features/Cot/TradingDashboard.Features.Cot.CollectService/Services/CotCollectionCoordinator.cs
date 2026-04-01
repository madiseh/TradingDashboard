using Microsoft.AspNetCore.WebUtilities;
using TradingDashboard.Features.Cot.CollectService.Abstractions;
using TradingDashboard.Features.Cot.CollectService.Models;
using TradingDashboard.Features.Cot.CollectService.Options;
using System.Globalization;

namespace TradingDashboard.Features.Cot.CollectService.Services;

public sealed class CotCollectionCoordinator : ICotCollectionCoordinator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CotCollectionCoordinator> _logger;
    private readonly CotCollectorOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly Dictionary<string, DateOnly> _datasetLatestReportDateCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, IReadOnlyDictionary<string, string>> _datasetLatestAliasCache = new(StringComparer.OrdinalIgnoreCase);

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
            var includeHistoricalBackfill = ShouldBackfillHistory(_lastTriggerReason);

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
                    var result = await CollectReportsAsync(
                        httpClient,
                        profile,
                        profile.NormalMarketAndExchangeName,
                        _options.Normal,
                        CotReportModel.Cot,
                        storageRoot,
                        includeHistoricalBackfill,
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
                    var result = await CollectReportsAsync(
                        httpClient,
                        profile,
                        profile.LegacyMarketAndExchangeName,
                        _options.Legacy,
                        CotReportModel.CotLegacy,
                        storageRoot,
                        includeHistoricalBackfill,
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

    private bool ShouldBackfillHistory(string reason)
    {
        return string.Equals(reason, "startup", StringComparison.OrdinalIgnoreCase);
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

    private async Task<FetchResult> CollectReportsAsync(
        HttpClient httpClient,
        CotInstrumentCollectionProfile profile,
        string marketAndExchangeName,
        CotCollectorModeOptions modeOptions,
        CotReportModel model,
        string storageRoot,
        bool includeHistoricalBackfill,
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
        var modelFolder = model == CotReportModel.Cot ? "normal" : "legacy";
        var targetDirectory = Path.Combine(storageRoot, modelFolder, instrumentSlug);
        Directory.CreateDirectory(targetDirectory);

        var historyStartDate = ResolveHistoryStartDate();
        var incrementalStartDate = await ResolveIncrementalStartDateAsync(targetDirectory, historyStartDate, cancellationToken);
        var startDate = includeHistoricalBackfill ? historyStartDate : incrementalStartDate;

        try
        {
            var offset = 0;
            var savedFileCount = 0;
            DateOnly? latestAvailableReportDate = null;
            string? latestAvailablePayload = null;
            var aliases = await ResolveMarketAliasesAsync(httpClient, marketAndExchangeName, modeOptions, cancellationToken);
            var processedDates = new HashSet<DateOnly>();

            foreach (var alias in aliases)
            {
                offset = 0;

                while (true)
                {
                    var requestUri = BuildRequestUri(alias, modeOptions, startDate, offset);
                    using var response = await httpClient.GetAsync(requestUri, cancellationToken);
                    var payload = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        return FetchResult.Failure(
                            $"Fetch failed for '{instrumentSlug}' ({model}) with status {(int)response.StatusCode}: {payload}");
                    }

                    using var document = JsonDocument.Parse(payload);
                    if (document.RootElement.ValueKind != JsonValueKind.Array)
                    {
                        return FetchResult.Failure($"Unexpected payload shape for '{instrumentSlug}' ({model}).");
                    }

                    var batchCount = 0;
                    foreach (var item in document.RootElement.EnumerateArray())
                    {
                        batchCount++;

                        if (!TryReadReportDate(item, out var reportDate))
                        {
                            return FetchResult.Failure(
                                $"Payload for '{instrumentSlug}' ({model}) does not contain a valid report date.");
                        }

                        var singleReportPayload = $"[{item.GetRawText()}]";
                        if (latestAvailableReportDate is null || reportDate > latestAvailableReportDate.Value)
                        {
                            latestAvailableReportDate = reportDate;
                            latestAvailablePayload = singleReportPayload;
                        }

                        if (!processedDates.Add(reportDate))
                        {
                            continue;
                        }

                        if (await WriteReportFileIfMissingAsync(targetDirectory, reportDate, singleReportPayload, cancellationToken))
                        {
                            savedFileCount++;
                            _logger.LogInformation(
                                "Saved raw COT payload for {InstrumentSlug} ({Model}) on {ReportDate} using alias {MarketAlias}",
                                instrumentSlug,
                                model,
                                reportDate,
                                alias);
                        }
                    }

                    if (batchCount == 0)
                    {
                        break;
                    }

                    if (batchCount < modeOptions.Limit)
                    {
                        break;
                    }

                    offset += batchCount;
                }
            }

            if (latestAvailablePayload is null)
            {
                return FetchResult.Success(0);
            }

            await WriteLatestFileAsync(targetDirectory, latestAvailablePayload, cancellationToken);
            _logger.LogInformation(
                "Updated latest raw COT payload for {InstrumentSlug} ({Model}) to report date {ReportDate}",
                instrumentSlug,
                model,
                latestAvailableReportDate);

            return FetchResult.Success(savedFileCount);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return FetchResult.Failure($"Fetch failed for '{instrumentSlug}' ({model}): {ex.Message}");
        }
    }

    private async Task<IReadOnlyList<string>> ResolveMarketAliasesAsync(
        HttpClient httpClient,
        string marketAndExchangeName,
        CotCollectorModeOptions modeOptions,
        CancellationToken cancellationToken)
    {
        var aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            marketAndExchangeName
        };

        var contractCode = await TryResolveContractCodeAsync(httpClient, marketAndExchangeName, modeOptions, cancellationToken);
        if (string.IsNullOrWhiteSpace(contractCode))
        {
            return aliases.ToArray();
        }

        var latestAliases = await GetLatestAliasesByContractCodeAsync(httpClient, modeOptions, cancellationToken);
        if (latestAliases.TryGetValue(contractCode, out var latestAlias) &&
            !string.IsNullOrWhiteSpace(latestAlias))
        {
            aliases.Add(latestAlias);
        }

        return aliases.ToArray();
    }

    private async Task<string?> TryResolveContractCodeAsync(
        HttpClient httpClient,
        string marketAndExchangeName,
        CotCollectorModeOptions modeOptions,
        CancellationToken cancellationToken)
    {
        var query = new Dictionary<string, string?>
        {
            ["$where"] = $"market_and_exchange_names='{EscapeSingleQuotes(marketAndExchangeName)}'",
            ["$order"] = "report_date_as_yyyy_mm_dd DESC",
            ["$limit"] = "1"
        };

        var requestUri = QueryHelpers.AddQueryString($"/resource/{modeOptions.Dataset}.json", query);
        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(payload);
        if (document.RootElement.ValueKind != JsonValueKind.Array || document.RootElement.GetArrayLength() == 0)
        {
            return null;
        }

        var firstItem = document.RootElement[0];
        return firstItem.TryGetProperty("cftc_contract_market_code", out var contractCodeElement)
            ? contractCodeElement.GetString()
            : null;
    }

    private async Task<IReadOnlyDictionary<string, string>> GetLatestAliasesByContractCodeAsync(
        HttpClient httpClient,
        CotCollectorModeOptions modeOptions,
        CancellationToken cancellationToken)
    {
        if (_datasetLatestAliasCache.TryGetValue(modeOptions.Dataset, out var cachedAliases))
        {
            return cachedAliases;
        }

        var latestReportDate = await GetLatestDatasetReportDateAsync(httpClient, modeOptions, cancellationToken);
        var query = new Dictionary<string, string?>
        {
            ["$where"] = $"report_date_as_yyyy_mm_dd = '{latestReportDate:yyyy-MM-dd}T00:00:00'",
            ["$limit"] = "5000"
        };

        var requestUri = QueryHelpers.AddQueryString($"/resource/{modeOptions.Dataset}.json", query);
        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(payload);

        var aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in document.RootElement.EnumerateArray())
            {
                if (!item.TryGetProperty("cftc_contract_market_code", out var contractCodeElement) ||
                    !item.TryGetProperty("market_and_exchange_names", out var aliasElement))
                {
                    continue;
                }

                var contractCode = contractCodeElement.GetString();
                var alias = aliasElement.GetString();
                if (string.IsNullOrWhiteSpace(contractCode) || string.IsNullOrWhiteSpace(alias))
                {
                    continue;
                }

                aliases[contractCode] = alias;
            }
        }

        _datasetLatestAliasCache[modeOptions.Dataset] = aliases;
        return aliases;
    }

    private async Task<DateOnly> GetLatestDatasetReportDateAsync(
        HttpClient httpClient,
        CotCollectorModeOptions modeOptions,
        CancellationToken cancellationToken)
    {
        if (_datasetLatestReportDateCache.TryGetValue(modeOptions.Dataset, out var cachedDate))
        {
            return cachedDate;
        }

        var query = new Dictionary<string, string?>
        {
            ["$select"] = "max(report_date_as_yyyy_mm_dd) as latest_report_date",
            ["$limit"] = "1"
        };

        var requestUri = QueryHelpers.AddQueryString($"/resource/{modeOptions.Dataset}.json", query);
        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(payload);
        if (document.RootElement.ValueKind != JsonValueKind.Array || document.RootElement.GetArrayLength() == 0)
        {
            throw new InvalidOperationException($"Could not determine latest report date for dataset '{modeOptions.Dataset}'.");
        }

        var firstItem = document.RootElement[0];
        if (!firstItem.TryGetProperty("latest_report_date", out var latestDateElement))
        {
            throw new InvalidOperationException($"Dataset '{modeOptions.Dataset}' did not return latest_report_date.");
        }

        var latestDateValue = latestDateElement.GetString();
        if (string.IsNullOrWhiteSpace(latestDateValue) ||
            !DateOnly.TryParseExact(
                latestDateValue[..10],
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var latestReportDate))
        {
            throw new InvalidOperationException($"Dataset '{modeOptions.Dataset}' returned an invalid latest_report_date.");
        }

        _datasetLatestReportDateCache[modeOptions.Dataset] = latestReportDate;
        return latestReportDate;
    }

    private string BuildRequestUri(
        string marketAndExchangeName,
        CotCollectorModeOptions modeOptions,
        DateOnly startDate,
        int offset)
    {
        var filters = new List<string>
        {
            $"market_and_exchange_names='{EscapeSingleQuotes(marketAndExchangeName)}'",
            $"report_date_as_yyyy_mm_dd >= '{startDate:yyyy-MM-dd}T00:00:00'"
        };

        var query = new Dictionary<string, string?>
        {
            ["$where"] = string.Join(" AND ", filters),
            ["$order"] = "report_date_as_yyyy_mm_dd ASC",
            ["$limit"] = modeOptions.Limit.ToString()
        };

        if (offset > 0)
        {
            query["$offset"] = offset.ToString(CultureInfo.InvariantCulture);
        }

        return QueryHelpers.AddQueryString($"/resource/{modeOptions.Dataset}.json", query);
    }

    private string ResolveStorageRoot()
    {
        return Path.IsPathRooted(_options.StorageRoot)
            ? _options.StorageRoot
            : Path.Combine(AppContext.BaseDirectory, _options.StorageRoot);
    }

    private DateOnly ResolveHistoryStartDate()
    {
        if (DateOnly.TryParseExact(
            _options.HistoryStartDate,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsedDate))
        {
            return parsedDate;
        }

        throw new InvalidOperationException("CotCollector:HistoryStartDate must be in yyyy-MM-dd format.");
    }

    private async Task<DateOnly> ResolveIncrementalStartDateAsync(
        string targetDirectory,
        DateOnly historyStartDate,
        CancellationToken cancellationToken)
    {
        var latestStoredDate = await TryReadLatestStoredReportDateAsync(targetDirectory, cancellationToken);
        if (latestStoredDate is null)
        {
            return historyStartDate;
        }

        var lookbackDays = Math.Max(_options.IncrementalLookbackDays, 0);
        var incrementalStartDate = latestStoredDate.Value.AddDays(-lookbackDays);
        return incrementalStartDate < historyStartDate ? historyStartDate : incrementalStartDate;
    }

    private async Task<DateOnly?> TryReadLatestStoredReportDateAsync(string targetDirectory, CancellationToken cancellationToken)
    {
        var latestFilePath = Path.Combine(targetDirectory, "latest.json");
        if (File.Exists(latestFilePath))
        {
            var latestDate = await TryReadReportDateFromFileAsync(latestFilePath, cancellationToken);
            if (latestDate is not null)
            {
                return latestDate;
            }
        }

        if (!Directory.Exists(targetDirectory))
        {
            return null;
        }

        return Directory.EnumerateFiles(targetDirectory, "*.json", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileNameWithoutExtension)
            .Where(fileName => !string.Equals(fileName, "latest", StringComparison.OrdinalIgnoreCase))
            .Select(fileName =>
                DateOnly.TryParseExact(fileName, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
                    ? parsedDate
                    : (DateOnly?)null)
            .Where(parsedDate => parsedDate is not null)
            .Max();
    }

    private static async Task<DateOnly?> TryReadReportDateFromFileAsync(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = File.OpenRead(filePath);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            if (document.RootElement.ValueKind != JsonValueKind.Array || document.RootElement.GetArrayLength() == 0)
            {
                return null;
            }

            return TryReadReportDate(document.RootElement[0], out var reportDate)
                ? reportDate
                : null;
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            return null;
        }
    }

    private static bool TryReadReportDate(JsonElement element, out DateOnly reportDate)
    {
        reportDate = default;

        if (element.ValueKind != JsonValueKind.Object ||
            !element.TryGetProperty("report_date_as_yyyy_mm_dd", out var rawReportDateElement))
        {
            return false;
        }

        var rawReportDate = rawReportDateElement.GetString();
        if (string.IsNullOrWhiteSpace(rawReportDate) || rawReportDate.Length < 10)
        {
            return false;
        }

        return DateOnly.TryParseExact(
            rawReportDate[..10],
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out reportDate);
    }

    private static async Task<bool> WriteReportFileIfMissingAsync(
        string targetDirectory,
        DateOnly reportDate,
        string payload,
        CancellationToken cancellationToken)
    {
        var reportFilePath = BuildReportFilePath(targetDirectory, reportDate);
        if (File.Exists(reportFilePath))
        {
            return false;
        }

        await File.WriteAllTextAsync(reportFilePath, payload, cancellationToken);
        return true;
    }

    private static Task WriteLatestFileAsync(string targetDirectory, string payload, CancellationToken cancellationToken)
    {
        var latestFilePath = Path.Combine(targetDirectory, "latest.json");
        return File.WriteAllTextAsync(latestFilePath, payload, cancellationToken);
    }

    private static string BuildReportFilePath(string targetDirectory, DateOnly reportDate)
    {
        return Path.Combine(targetDirectory, $"{reportDate:yyyy-MM-dd}.json");
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
        public static FetchResult Success(int savedFiles) => new(true, savedFiles, null);
        public static FetchResult Failure(string errorMessage) => new(false, 0, errorMessage);
    }
}
