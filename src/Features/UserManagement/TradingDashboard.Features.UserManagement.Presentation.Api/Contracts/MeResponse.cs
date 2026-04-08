namespace TradingDashboard.Presentation.Api.UserManagement.Contracts;

public sealed record MeResponse(
    string? UserId,
    string? Username,
    string? Email,
    string? Role);
