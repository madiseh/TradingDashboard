using TradingDashboard.Application.Common.Models;

namespace TradingDashboard.Application.Repository.Cot;

/// <summary>
/// Reads historical collected raw COT reports for charting and date selection.
/// </summary>
public interface ICotHistoricalReportQueryRepository
{
    /// <summary>
    /// Returns every available report date for an instrument/model pair.
    /// </summary>
    /// <param name="instrumentSlug">Instrument slug.</param>
    /// <param name="model">Requested report model.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ordered available report dates.</returns>
    Task<IReadOnlyList<DateOnly>> GetAvailableDatesAsync(
        string instrumentSlug,
        CotReportModel model,
        CancellationToken cancellationToken);

    /// <summary>
    /// Returns collected report rows inside the requested date range for an instrument/model pair.
    /// </summary>
    /// <param name="instrumentSlug">Instrument slug.</param>
    /// <param name="model">Requested report model.</param>
    /// <param name="fromDate">Inclusive start date.</param>
    /// <param name="toDate">Inclusive end date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ordered historical report records.</returns>
    Task<IReadOnlyList<CotLatestReportRawRecord>> GetRangeAsync(
        string instrumentSlug,
        CotReportModel model,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken);
}
