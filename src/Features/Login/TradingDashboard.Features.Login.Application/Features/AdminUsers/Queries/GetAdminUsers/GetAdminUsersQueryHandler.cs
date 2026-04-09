using MediatR;
using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.AdminUsers.Queries.GetAdminUsers;

public sealed class GetAdminUsersQueryHandler(IAppUserQueryRepository userQueryRepository)
    : IRequestHandler<GetAdminUsersQuery, GetAdminUsersResponse>
{
    public async Task<GetAdminUsersResponse> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userQueryRepository.SearchAsync(request.Search, cancellationToken);

        return new GetAdminUsersResponse(
            users.Count,
            users.Count(x => x.Status == AppUserStatus.Active),
            users.Count(x => x.Status == AppUserStatus.Disabled),
            users.Count(x => x.Role == AppUserRole.SuperAdmin),
            users.Select(user => new AdminUserItemResponse(
                    user.Id,
                    user.Username,
                    user.Email.Value,
                    user.Role.ToString(),
                    user.Status.ToString(),
                    user.SignupAppPlatform.ToString(),
                    user.IsEmailVerified,
                    user.CreatedAtUtc,
                    user.UpdatedAtUtc,
                    user.LastLoginAtUtc))
                .ToList());
    }
}
