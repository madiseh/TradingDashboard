namespace TradingDashboard.Presentation.Api.Login.Options;

public sealed class AdminSeedOptions
{
    public const string SectionName = "AdminSeed";

    public bool Enabled { get; init; } = true;
    public string Username { get; init; } = "admin";
    public string Email { get; init; } = "admin@tradingdashboard.com";
    public string Password { get; init; } = "Admin@12345";
}
