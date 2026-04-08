namespace TradingDashboard.Presentation.Api.Login.Contracts;

public sealed record MeResponse(
    string? UserId,
    string? Username,
    string? Email,
    string? Role);
