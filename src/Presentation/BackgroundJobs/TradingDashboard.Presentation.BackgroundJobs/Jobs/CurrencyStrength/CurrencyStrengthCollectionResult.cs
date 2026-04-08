namespace TradingDashboard.Presentation.BackgroundJobs.Jobs.CurrencyStrength;

public sealed record CurrencyStrengthCollectionResult(
    int SavedFileCount,
    int SuccessfulFetchCount,
    string? Error);
