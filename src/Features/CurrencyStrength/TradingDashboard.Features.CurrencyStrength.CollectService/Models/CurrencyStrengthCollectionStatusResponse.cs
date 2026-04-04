namespace TradingDashboard.Features.CurrencyStrength.CollectService.Models;

public sealed record CurrencyStrengthCollectionStatusResponse(
    bool IsRunning,
    string LastTriggerReason,
    DateTime? CurrentRunStartedAtUtc,
    DateTime? NextScheduledRunAtUtc,
    DateTime? LastCompletedAtUtc,
    DateTime? LastSucceededAtUtc,
    int LastSavedFileCount,
    int LastSuccessfulFetchCount,
    string? LastError,
    int IntervalMinutes);
