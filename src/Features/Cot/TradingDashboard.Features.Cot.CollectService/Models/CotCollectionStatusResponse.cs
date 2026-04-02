namespace TradingDashboard.Features.Cot.CollectService.Models;

public sealed record CotCollectionStatusResponse(
    bool IsRunning,
    string LastTriggerReason,
    DateTime? CurrentRunStartedAtUtc,
    DateTime? NextScheduledRunAtUtc,
    DateTime? LastStartedAtUtc,
    DateTime? LastCompletedAtUtc,
    DateTime? LastSucceededAtUtc,
    int LastSavedFileCount,
    int LastFetchedProfileCount,
    string? LastError,
    int ConfiguredIntervalMinutes);
