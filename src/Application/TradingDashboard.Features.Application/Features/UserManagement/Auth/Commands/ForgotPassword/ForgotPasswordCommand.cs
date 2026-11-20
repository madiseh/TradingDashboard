using MediatR;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Identity)
    : IRequest<OperationalResult<AuthActionResponse>>;
