using TradingDashboard.Application.Features.Auth;

namespace TradingDashboard.Presentation.Api.UserManagement.Services;

public interface IJwtTokenService
{
    AuthTokenResult GenerateToken(AuthUserResponse user);
}

public sealed record AuthTokenResult(
    string AccessToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Username,
    string Email);
