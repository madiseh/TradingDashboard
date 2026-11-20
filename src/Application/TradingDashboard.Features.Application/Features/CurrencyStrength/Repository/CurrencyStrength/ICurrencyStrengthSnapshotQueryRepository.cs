using TradingDashboard.Features.CurrencyStrength.Domain.ValueObjects;

namespace TradingDashboard.Application.Repository.CurrencyStrength;

/// <summary>
/// Reads the latest collected currency-strength snapshot for a supported timeframe.
/// </summary>
public interface ICurrencyStrengthSnapshotQueryRepository
{
    /// <summary>
    /// Returns the latest collected snapshot for the requested timeframe.
    /// </summary>
    Task<CurrencyStrengthSnapshot?> GetLatestAsync(string timeframeKey, CancellationToken cancellationToken);
}
