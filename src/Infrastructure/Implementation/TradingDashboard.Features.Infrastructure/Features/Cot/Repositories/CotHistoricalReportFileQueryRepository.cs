using TradingDashboard.Application.Common.Models;
using TradingDashboard.Application.Repository.Cot;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

public sealed class CotHistoricalReportFileQueryRepository : ICotHistoricalReportQueryRepository
{
    private readonly IReadOnlyList<string> _storageRoots;

    public CotHistoricalReportFileQueryRepository(IReadOnlyList<string> storageRoots)
    {
        _storageRoots = storageRoots;
    }

    public Task<IReadOnlyList<DateOnly>> GetAvailableDatesAsync(
        string instrumentSlug,
        CotReportModel model,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(instrumentSlug))
        {
            throw new ArgumentException("Instrument slug cannot be empty.", nameof(instrumentSlug));
        }

        var availableDates = CotRawReportFileReader
            .GetInstrumentDirectories(_storageRoots, instrumentSlug, model)
            .SelectMany(CotRawReportFileReader.EnumerateReportFiles)
            .Select(x => x.ReportDate)
            .Distinct()
            .OrderBy(x => x)
            .ToArray();

        return Task.FromResult<IReadOnlyList<DateOnly>>(availableDates);
    }

    public async Task<IReadOnlyList<CotLatestReportRawRecord>> GetRangeAsync(
        string instrumentSlug,
        CotReportModel model,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(instrumentSlug))
        {
            throw new ArgumentException("Instrument slug cannot be empty.", nameof(instrumentSlug));
        }

        if (fromDate > toDate)
        {
            throw new ArgumentException("'fromDate' must be less than or equal to 'toDate'.", nameof(fromDate));
        }

        var reportFiles = CotRawReportFileReader
            .GetInstrumentDirectories(_storageRoots, instrumentSlug, model)
            .SelectMany(CotRawReportFileReader.EnumerateReportFiles)
            .Where(x => x.ReportDate >= fromDate && x.ReportDate <= toDate)
            .GroupBy(x => x.ReportDate)
            .Select(x => x.First())
            .OrderBy(x => x.ReportDate)
            .ToArray();

        var records = new List<CotLatestReportRawRecord>(reportFiles.Length);
        foreach (var reportFile in reportFiles)
        {
            var record = await CotRawReportFileReader.ReadRecordAsync(
                reportFile.Path,
                reportFile.ReportDate,
                cancellationToken);

            if (record is not null)
            {
                records.Add(record);
            }
        }

        return records;
    }
}
