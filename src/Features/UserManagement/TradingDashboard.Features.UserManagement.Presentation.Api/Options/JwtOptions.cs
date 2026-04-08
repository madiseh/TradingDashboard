namespace TradingDashboard.Presentation.Api.UserManagement.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "TradingDashboard.UserManagementApi";
    public string Audience { get; init; } = "TradingDashboard.Blazor";
    public string SigningKey { get; init; } = "trading-dashboard-local-dev-signing-key-change-me";
    public int AccessTokenLifetimeMinutes { get; init; } = 60;
}
