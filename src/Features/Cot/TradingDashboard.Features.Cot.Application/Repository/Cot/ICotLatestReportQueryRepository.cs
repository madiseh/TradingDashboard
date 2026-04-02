using TradingDashboard.Application.Common.Models;

namespace TradingDashboard.Application.Repository.Cot;

/// <summary>
/// Reads a collected raw COT report for an instrument and report model.
/// </summary>
public interface ICotLatestReportQueryRepository
{
    /// <summary>
    /// Returns the collected raw report entry for the given instrument slug, model, and optional report date.
    /// </summary>
    /// <param name="instrumentSlug">Instrument slug.</param>
    /// <param name="model">Requested report model.</param>
    /// <param name="reportDate">Requested report date. When null, the latest report is returned.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The collected report entry when present; otherwise null.</returns>
    Task<CotLatestReportRawRecord?> GetAsync(
        string instrumentSlug,
        CotReportModel model,
        DateOnly? reportDate,
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
