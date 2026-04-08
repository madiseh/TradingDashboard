using MediatR;

namespace TradingDashboard.Application.Features.AdminUsers.Queries.GetAdminUsers;

public sealed record GetAdminUsersQuery(string? Search)
    : IRequest<GetAdminUsersResponse>;
