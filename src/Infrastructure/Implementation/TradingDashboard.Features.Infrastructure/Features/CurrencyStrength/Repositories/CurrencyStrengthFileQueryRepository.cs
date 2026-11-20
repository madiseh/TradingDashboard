using System.Text.Json;
using TradingDashboard.Application.Repository.CurrencyStrength;
using TradingDashboard.Features.CurrencyStrength.Domain.Catalog;
using TradingDashboard.Features.CurrencyStrength.Domain.ValueObjects;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

public sealed class CurrencyStrengthFileQueryRepository : ICurrencyStrengthSnapshotQueryRepository
{
    private readonly IReadOnlyList<string> _storageRoots;

    public CurrencyStrengthFileQueryRepository(IReadOnlyList<string> storageRoots)
    {
        _storageRoots = storageRoots;
    }

    public async Task<CurrencyStrengthSnapshot?> GetLatestAsync(string timeframeKey, CancellationToken cancellationToken)
    {
        if (!CurrencyStrengthCatalog.TryGet(timeframeKey, out var timeframe))
        {
            throw new ArgumentException("Unsupported timeframe.", nameof(timeframeKey));
        }

        foreach (var storageRoot in _storageRoots)
        {
            var timeframeDirectory = Path.Combine(storageRoot, timeframe.Key);
            if (!Directory.Exists(timeframeDirectory))
            {
                continue;
            }

            var latestFilePath = Path.Combine(timeframeDirectory, "latest.json");
            if (File.Exists(latestFilePath))
            {
                var snapshot = await TryReadSnapshotAsync(latestFilePath, timeframe, cancellationToken);
                if (snapshot is not null)
                {
                    return snapshot;
                }
            }

            var fallbackSnapshot = await TryReadLatestSnapshotBackupAsync(timeframeDirectory, timeframe, cancellationToken);
            if (fallbackSnapshot is not null)
            {
                return fallbackSnapshot;
            }
        }

        return null;
    }

    private static async Task<CurrencyStrengthSnapshot?> TryReadSnapshotAsync(
        string filePath,
        CurrencyStrengthTimeframeDefinition timeframe,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete,
                bufferSize: 4096,
                options: FileOptions.Asynchronous | FileOptions.SequentialScan);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var series = new List<CurrencyStrengthSeries>();
            foreach (var item in document.RootElement.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object ||
                    !item.TryGetProperty("key", out var keyElement) ||
                    !item.TryGetProperty("values", out var valuesElement) ||
                    valuesElement.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                var currencyCode = keyElement.GetString();
                if (string.IsNullOrWhiteSpace(currencyCode))
                {
                    continue;
                }

                var points = new List<CurrencyStrengthPoint>();
                foreach (var valueItem in valuesElement.EnumerateArray())
                {
                    if (valueItem.ValueKind != JsonValueKind.Array || valueItem.GetArrayLength() < 2)
                    {
                        continue;
                    }

                    if (!TryReadInt64(valueItem[0], out var timestamp) ||
                        !TryReadDecimal(valueItem[1], out var value))
                    {
                        continue;
                    }

                    points.Add(new CurrencyStrengthPoint(timestamp, value));
                }

                if (points.Count > 0)
                {
                    series.Add(new CurrencyStrengthSeries(currencyCode.Trim().ToUpperInvariant(), points));
                }
            }

            return series.Count == 0
                ? null
                : new CurrencyStrengthSnapshot(timeframe.Key, timeframe.Label, File.GetLastWriteTimeUtc(filePath), series);
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            return null;
        }
    }

    private static async Task<CurrencyStrengthSnapshot?> TryReadLatestSnapshotBackupAsync(
        string timeframeDirectory,
        CurrencyStrengthTimeframeDefinition timeframe,
        CancellationToken cancellationToken)
    {
        var snapshotsDirectory = Path.Combine(timeframeDirectory, "snapshots");
        if (!Directory.Exists(snapshotsDirectory))
        {
            return null;
        }

        var candidateFiles = Directory
            .EnumerateFiles(snapshotsDirectory, "*.json", SearchOption.AllDirectories)
            .OrderByDescending(File.GetLastWriteTimeUtc);

        foreach (var candidateFile in candidateFiles)
        {
            var snapshot = await TryReadSnapshotAsync(candidateFile, timeframe, cancellationToken);
            if (snapshot is not null)
            {
                return snapshot;
            }
        }

        return null;
    }

    private static bool TryReadInt64(JsonElement element, out long value)
    {
        if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out value))
        {
            return true;
        }

        if (element.ValueKind == JsonValueKind.String &&
            long.TryParse(element.GetString(), out value))
        {
            return true;
        }

        value = default;
        return false;
    }

    private static bool TryReadDecimal(JsonElement element, out decimal value)
    {
        if (element.ValueKind == JsonValueKind.Number && element.TryGetDecimal(out value))
        {
            return true;
        }

        if (element.ValueKind == JsonValueKind.String &&
            decimal.TryParse(element.GetString(), out value))
        {
            return true;
        }

        value = default;
        return false;
    }
}
