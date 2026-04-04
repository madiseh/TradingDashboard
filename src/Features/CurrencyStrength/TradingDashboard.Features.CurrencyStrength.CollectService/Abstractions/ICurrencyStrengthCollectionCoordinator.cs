using TradingDashboard.Features.CurrencyStrength.CollectService.Models;

namespace TradingDashboard.Features.CurrencyStrength.CollectService.Abstractions;

public interface ICurrencyStrengthCollectionCoordinator
{
    Task<CurrencyStrengthCollectionStatusResponse> GetStatusAsync(CancellationToken cancellationToken);

    Task<CurrencyStrengthCollectionStatusResponse> RunOnceAsync(string reason, CancellationToken cancellationToken);

    void SetNextScheduledRun(DateTime? nextScheduledRunAtUtc);
}
