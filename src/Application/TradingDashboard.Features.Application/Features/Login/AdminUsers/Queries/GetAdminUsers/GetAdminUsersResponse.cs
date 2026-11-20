namespace TradingDashboard.Application.Features.AdminUsers.Queries.GetAdminUsers;

public sealed record GetAdminUsersResponse(
    int TotalUsers,
    int ActiveUsers,
    int DisabledUsers,
    int SuperAdminUsers,
    IReadOnlyList<AdminUserItemResponse> Users);

public sealed record AdminUserItemResponse(
    Guid UserId,
    string Username,
    string Email,
    string Role,
    string Status,
    string SignupPlatform,
    bool IsEmailVerified,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    DateTime? LastLoginAtUtc);
