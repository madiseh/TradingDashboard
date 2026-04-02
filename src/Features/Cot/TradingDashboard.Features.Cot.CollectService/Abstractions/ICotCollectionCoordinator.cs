namespace TradingDashboard.Features.Cot.CollectService.Abstractions;

/// <summary>
/// Coordinates scheduled and on-demand COT raw-data collection runs.
/// </summary>
public interface ICotCollectionCoordinator
{
    /// <summary>
    /// Returns the latest runtime and persisted status for the collector.
    /// </summary>
    Task<CotCollectionStatusResponse> GetStatusAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Executes a collection cycle immediately.
    /// </summary>
    Task<CotCollectionStatusResponse> RunOnceAsync(string reason, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the next scheduled run timestamp exposed by the status endpoint.
    /// </summary>
    void SetNextScheduledRun(DateTime? nextScheduledRunAtUtc);
}
