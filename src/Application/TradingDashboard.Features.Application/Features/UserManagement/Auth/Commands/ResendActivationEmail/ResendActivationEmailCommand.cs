using MediatR;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.Auth.Commands.ResendActivationEmail;

public sealed record ResendActivationEmailCommand(string Identity)
    : IRequest<OperationalResult<AuthActionResponse>>;
