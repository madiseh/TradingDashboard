namespace TradingDashboard.Presentation.Api.Login.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; init; } = "Sqlite";
}
