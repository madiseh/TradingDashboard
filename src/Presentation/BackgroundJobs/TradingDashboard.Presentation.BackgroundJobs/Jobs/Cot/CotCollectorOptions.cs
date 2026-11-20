namespace TradingDashboard.Presentation.BackgroundJobs.Jobs.Cot;

public sealed class CotCollectorOptions
{
    public const string SectionName = "CotCollector";

    public string BaseUrl { get; set; } = "https://publicreporting.cftc.gov";
    public string StorageRoot { get; set; } = Path.Combine("storage", "cot-raw");
    public string HistoryStartDate { get; set; } = "2010-01-01";
    public int IncrementalLookbackDays { get; set; } = 7;
    public int RequestTimeoutSeconds { get; set; } = 30;
    public bool RunOnStartup { get; set; } = true;
    public int IntervalMinutes { get; set; } = 10;
    public CotCollectorModeOptions Normal { get; set; } = new();
    public CotCollectorModeOptions Disaggregated { get; set; } = new();
    public CotCollectorModeOptions Legacy { get; set; } = new();
}

public sealed class CotCollectorModeOptions
{
    public bool Enabled { get; set; } = true;
    public string Dataset { get; set; } = string.Empty;
    public int Limit { get; set; }
}
