using System.Text.Json;
using TradingDashboard.Application.Common.Models;
using TradingDashboard.Application.Repository.Cot;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

public sealed class CotLatestReportFileQueryRepository : ICotLatestReportQueryRepository
{
    private readonly IReadOnlyList<string> _storageRoots;

    public CotLatestReportFileQueryRepository(IReadOnlyList<string> storageRoots)
    {
        _storageRoots = storageRoots;
    }

    public async Task<CotLatestReportRawRecord?> GetLatestAsync(
        string instrumentSlug,
        CotReportModel model,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(instrumentSlug))
        {
            throw new ArgumentException("Instrument slug cannot be empty.", nameof(instrumentSlug));
        }

        var normalizedSlug = instrumentSlug.Trim();
        var modelFolder = model == CotReportModel.CotLegacy ? "legacy" : "normal";

        foreach (var storageRoot in _storageRoots)
        {
            var latestFilePath = Path.Combine(storageRoot, modelFolder, normalizedSlug, "latest.json");
            if (!File.Exists(latestFilePath))
            {
                continue;
            }

            await using var stream = File.OpenRead(latestFilePath);
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

            return new CotLatestReportRawRecord(
                marketAndExchangeNames,
                reportDate,
                contractUnits,
                fields);
        }

        return null;
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
}
