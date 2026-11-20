using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.Auth;

public sealed record AuthUserResponse(Guid UserId, string Username, string Email, AppUserRole Role);
