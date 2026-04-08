namespace TradingDashboard.Presentation.BackgroundJobs.Jobs.CurrencyStrength;

public sealed class CurrencyStrengthCollectorOptions
{
    public const string SectionName = "CurrencyStrengthCollector";

    public string BaseUrl { get; set; } = "https://fx-chart.net/base_api/strength_base.php";
    public string StorageRoot { get; set; } = Path.Combine("storage", "currency-strength");
    public int RequestTimeoutSeconds { get; set; } = 30;
    public bool RunOnStartup { get; set; } = true;
    public int IntervalMinutes { get; set; } = 1;
    public int SnapshotRetentionDays { get; set; } = 14;
    public List<CurrencyStrengthCollectorTimeframeOptions> Timeframes { get; set; } = [];
}

public sealed class CurrencyStrengthCollectorTimeframeOptions
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string QueryValue { get; set; } = string.Empty;
}
