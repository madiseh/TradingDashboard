namespace TradingDashboard.Application.Features.AdminUsers.Commands.ChangeUserPassword;

public sealed record ChangeUserPasswordResponse(Guid UserId, string Message);
