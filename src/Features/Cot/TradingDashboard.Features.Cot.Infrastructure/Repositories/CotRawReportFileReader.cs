using System.Globalization;
using System.Text.Json;
using TradingDashboard.Application.Common.Models;
using TradingDashboard.Application.Repository.Cot;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

internal static class CotRawReportFileReader
{
    public static IEnumerable<string> GetInstrumentDirectories(
        IReadOnlyList<string> storageRoots,
        string instrumentSlug,
        CotReportModel model)
    {
        var normalizedSlug = instrumentSlug.Trim();
        var modelFolder = model == CotReportModel.CotLegacy ? "legacy" : "normal";

        foreach (var storageRoot in storageRoots)
        {
            var instrumentDirectory = Path.Combine(storageRoot, modelFolder, normalizedSlug);
            if (Directory.Exists(instrumentDirectory))
            {
                yield return instrumentDirectory;
            }
        }
    }

    public static string BuildReportFilePath(string instrumentDirectory, DateOnly reportDate)
    {
        return Path.Combine(instrumentDirectory, $"{reportDate:yyyy-MM-dd}.json");
    }

    public static string? ResolveLatestDatedFilePath(string instrumentDirectory)
    {
        return EnumerateReportFiles(instrumentDirectory)
            .OrderByDescending(x => x.ReportDate)
            .Select(x => x.Path)
            .FirstOrDefault();
    }

    public static IEnumerable<ReportFileEntry> EnumerateReportFiles(string instrumentDirectory)
    {
        return Directory.EnumerateFiles(instrumentDirectory, "*.json", SearchOption.TopDirectoryOnly)
            .Where(path => !string.Equals(Path.GetFileName(path), "latest.json", StringComparison.OrdinalIgnoreCase))
            .Select(path => new
            {
                Path = path,
                FileName = Path.GetFileNameWithoutExtension(path)
            })
            .Select(item => new ReportFileEntry(
                item.Path,
                DateOnly.TryParseExact(
                    item.FileName,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var reportDate)
                    ? reportDate
                    : default,
                DateOnly.TryParseExact(
                    item.FileName,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _)))
            .Where(static item => item.IsValid)
            .Select(static item => new ReportFileEntry(item.Path, item.ReportDate));
    }

    public static async Task<CotLatestReportRawRecord?> ReadRecordAsync(
        string filePath,
        DateOnly? fallbackReportDate,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = File.OpenRead(filePath);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            if (document.RootElement.ValueKind != JsonValueKind.Array || document.RootElement.GetArrayLength() == 0)
            {
                return null;
            }

            var latestElement = document.RootElement[0];
            if (latestElement.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var fields = latestElement.EnumerateObject()
                .ToDictionary(
                    property => property.Name,
                    property => ReadValue(property.Value),
                    StringComparer.OrdinalIgnoreCase);

            fields.TryGetValue("market_and_exchange_names", out var marketAndExchangeNames);
            fields.TryGetValue("contract_units", out var contractUnits);

            DateTime? reportDate = null;
            if (fields.TryGetValue("report_date_as_yyyy_mm_dd", out var rawReportDate) &&
                DateTime.TryParse(rawReportDate, out var parsedReportDate))
            {
                reportDate = parsedReportDate;
            }
            else if (fallbackReportDate is not null)
            {
                reportDate = fallbackReportDate.Value.ToDateTime(TimeOnly.MinValue);
            }

            return new CotLatestReportRawRecord(
                marketAndExchangeNames,
                reportDate,
                contractUnits,
                fields);
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            return null;
        }
    }

    private static string? ReadValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.ToString(),
            JsonValueKind.True => bool.TrueString,
            JsonValueKind.False => bool.FalseString,
            _ => element.GetRawText()
        };
    }

    internal sealed record ReportFileEntry(string Path, DateOnly ReportDate, bool IsValid = true);
}
