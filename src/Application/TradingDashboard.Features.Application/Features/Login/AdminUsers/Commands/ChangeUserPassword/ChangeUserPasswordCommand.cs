using MediatR;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.AdminUsers.Commands.ChangeUserPassword;

public sealed record ChangeUserPasswordCommand(Guid UserId, string NewPassword)
    : IRequest<OperationalResult<ChangeUserPasswordResponse>>;
