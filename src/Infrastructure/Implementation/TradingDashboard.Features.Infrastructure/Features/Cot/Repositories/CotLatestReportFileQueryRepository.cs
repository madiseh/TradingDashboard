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

    public async Task<CotLatestReportRawRecord?> GetAsync(
        string instrumentSlug,
        CotReportModel model,
        DateOnly? reportDate,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(instrumentSlug))
        {
            throw new ArgumentException("Instrument slug cannot be empty.", nameof(instrumentSlug));
        }

        foreach (var instrumentDirectory in CotRawReportFileReader.GetInstrumentDirectories(_storageRoots, instrumentSlug, model))
        {
            if (reportDate is not null)
            {
                var requestedFilePath = CotRawReportFileReader.BuildReportFilePath(instrumentDirectory, reportDate.Value);
                if (!File.Exists(requestedFilePath))
                {
                    continue;
                }

                var requestedRecord = await CotRawReportFileReader.ReadRecordAsync(
                    requestedFilePath,
                    reportDate.Value,
                    cancellationToken);
                if (requestedRecord is not null)
                {
                    return requestedRecord;
                }

                continue;
            }

            var latestFilePath = Path.Combine(instrumentDirectory, "latest.json");
            if (File.Exists(latestFilePath))
            {
                var latestRecord = await CotRawReportFileReader.ReadRecordAsync(latestFilePath, null, cancellationToken);
                if (latestRecord is not null)
                {
                    return latestRecord;
                }
            }

            var fallbackFilePath = CotRawReportFileReader.ResolveLatestDatedFilePath(instrumentDirectory);
            if (fallbackFilePath is null)
            {
                continue;
            }

            var fallbackDate = CotRawReportFileReader.EnumerateReportFiles(instrumentDirectory)
                .FirstOrDefault(x => string.Equals(x.Path, fallbackFilePath, StringComparison.OrdinalIgnoreCase))
                ?.ReportDate;

            var fallbackRecord = await CotRawReportFileReader.ReadRecordAsync(
                fallbackFilePath,
                fallbackDate,
                cancellationToken);
            if (fallbackRecord is not null)
            {
                return fallbackRecord;
            }
        }

        return null;
    }
}
