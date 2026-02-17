namespace TradingDashboard.Presentation.Common.Contracts;

public sealed record ApiErrorResponse(
    string? Message,
    string? ErrorCode,
    IReadOnlyList<string>? Errors);
