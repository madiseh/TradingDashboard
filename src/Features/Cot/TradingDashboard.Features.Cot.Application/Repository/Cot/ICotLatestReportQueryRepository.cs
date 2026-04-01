using TradingDashboard.Application.Common.Models;

namespace TradingDashboard.Application.Repository.Cot;

/// <summary>
/// Reads the latest collected raw COT report for an instrument and report model.
/// </summary>
public interface ICotLatestReportQueryRepository
{
    /// <summary>
    /// Returns the latest collected raw report entry for the given instrument slug and model.
    /// </summary>
    /// <param name="instrumentSlug">Instrument slug.</param>
    /// <param name="model">Requested report model.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The latest collected report entry when present; otherwise null.</returns>
    Task<CotLatestReportRawRecord?> GetLatestAsync(
        string instrumentSlug,
        CotReportModel model,
        CancellationToken cancellationToken);
}

/// <summary>
/// Raw latest report record loaded from collector storage.
/// </summary>
/// <param name="MarketAndExchangeNames">Market and exchange label.</param>
/// <param name="ReportDate">Report date.</param>
/// <param name="ContractUnits">Contract units label.</param>
/// <param name="Fields">Flattened raw payload fields.</param>
public sealed record CotLatestReportRawRecord(
    string? MarketAndExchangeNames,
    DateTime? ReportDate,
    string? ContractUnits,
    IReadOnlyDictionary<string, string?> Fields);
