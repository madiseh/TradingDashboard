namespace TradingDashboard.Presentation.BackgroundJobs.Jobs.Cot;

public sealed record CotCollectionResult(
    int SavedFileCount,
    int SuccessfulFetchCount,
    string? Error);
